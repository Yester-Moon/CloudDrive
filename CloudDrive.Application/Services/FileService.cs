using CloudDrive.Application.Commands;
using CloudDrive.Application.Dtos;
using CloudDrive.Application.Interfaces;
using CloudDrive.Application.Queries;
using CloudDrive.Application.Validators;
using CloudDrive.Domain.Entities;
using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.RepositoryInterfaces;
using CloudDrive.Domain.ValueObjects;

namespace CloudDrive.Application.Services
{
    /// <summary>
    /// 文件应用服务
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStorageProvider _storageProvider;
        private readonly FileDeduplicationService _deduplicationService;
        private readonly FileUploadValidator _uploadValidator;
        private readonly QuotaService _quotaService;

        public FileService(
            IFileRepository fileRepository,
            IUserRepository userRepository,
            IStorageProvider storageProvider,
            FileDeduplicationService deduplicationService,
            FileUploadValidator uploadValidator,
            QuotaService quotaService)
        {
            _fileRepository = fileRepository;
            _userRepository = userRepository;
            _storageProvider = storageProvider;
            _deduplicationService = deduplicationService;
            _uploadValidator = uploadValidator;
            _quotaService = quotaService;
        }

        /// <inheritdoc />
        public async Task<FileUploadResultDto> UploadFileAsync(UploadFileCommand command)
        {
            // 计算文件哈希
            var hash = !string.IsNullOrWhiteSpace(command.FileHash)
                ? new FileHash(command.FileHash)
                : await _storageProvider.ComputeHashAsync(command.FileStream);

            // 如果流被读取过（计算哈希时），重置位置
            if (command.FileStream.CanSeek)
                command.FileStream.Position = 0;

            // 执行上传验证（文件类型、配额、去重）
            var validation = await _uploadValidator.ValidateAsync(
                command.OwnerId, command.FileName, command.FileSize, hash);

            if (!validation.IsValid)
            {
                if (validation.IsDuplicate)
                    return FileUploadResultDto.DuplicateFile(validation.ExistingFile!.Id, command.FileName);

                return FileUploadResultDto.Fail(validation.Message!);
            }

            // 如果可以秒传
            if (validation.CanInstantUpload && validation.ExistingFile != null)
            {
                var instantFile = await _deduplicationService.TryInstantUploadAsync(
                    hash, command.FileName, command.MimeType, command.OwnerId, command.ParentFolderId);

                if (instantFile != null)
                {
                    await _fileRepository.AddAsync(instantFile);

                    // 更新用户已用空间
                    var user = await _userRepository.GetByIdAsync(command.OwnerId);
                    user!.IncreaseUsedSpace(instantFile.Size.bytesize);
                    await _userRepository.UpdateAsync(user);

                    return FileUploadResultDto.Ok(instantFile.Id, command.FileName, instantFile.Size.bytesize, isInstantUpload: true);
                }
            }

            // 正常上传：存储文件
            var storagePath = await _storageProvider.UploadAsync(command.FileStream, command.FileName, command.MimeType);

            // 创建文件实体
            var fileItem = FileItem.CreateFile(
                command.FileName,
                new FileSize(command.FileSize),
                storagePath,
                hash,
                command.MimeType,
                command.OwnerId,
                command.ParentFolderId);

            await _fileRepository.AddAsync(fileItem);

            // 更新用户已用空间
            var owner = await _userRepository.GetByIdAsync(command.OwnerId);
            owner!.IncreaseUsedSpace(command.FileSize);
            await _userRepository.UpdateAsync(owner);

            return FileUploadResultDto.Ok(fileItem.Id, command.FileName, command.FileSize);
        }

        /// <inheritdoc />
        public async Task<FileUploadResultDto> TryInstantUploadAsync(
            Guid ownerId, string fileHash, string fileName, string mimeType, Guid? parentFolderId = null)
        {
            var hash = new FileHash(fileHash);

            // 验证上传条件
            var validation = await _uploadValidator.ValidateAsync(ownerId, fileName, 0, hash);
            if (validation.IsDuplicate)
                return FileUploadResultDto.DuplicateFile(validation.ExistingFile!.Id, fileName);

            // 尝试秒传
            var instantFile = await _deduplicationService.TryInstantUploadAsync(
                hash, fileName, mimeType, ownerId, parentFolderId);

            if (instantFile == null)
                return FileUploadResultDto.Fail("文件不存在于服务器，无法秒传");

            await _fileRepository.AddAsync(instantFile);

            // 更新用户已用空间
            var user = await _userRepository.GetByIdAsync(ownerId);
            user!.IncreaseUsedSpace(instantFile.Size.bytesize);
            await _userRepository.UpdateAsync(user);

            return FileUploadResultDto.Ok(instantFile.Id, fileName, instantFile.Size.bytesize, isInstantUpload: true);
        }

        /// <inheritdoc />
        public async Task<(Stream FileStream, string FileName, string MimeType)> DownloadFileAsync(Guid fileId, Guid userId)
        {
            var fileItem = await _fileRepository.GetByIdAsync(fileId)
                ?? throw new InvalidOperationException("文件不存在");

            if (fileItem.OwnerId != userId)
                throw new UnauthorizedAccessException("无权下载此文件");

            if (fileItem.IsFolder)
                throw new InvalidOperationException("文件夹不支持下载");

            var stream = await _storageProvider.DownloadAsync(fileItem.StoragePath);

            // 增加下载计数
            fileItem.IncrementDownloadCount();
            await _fileRepository.UpdateAsync(fileItem);

            return (stream, fileItem.Name, fileItem.MimeType);
        }

        /// <inheritdoc />
        public async Task<FileInfoDto?> GetFileInfoAsync(Guid fileId, Guid userId)
        {
            var fileItem = await _fileRepository.GetByIdAsync(fileId);
            if (fileItem == null || fileItem.OwnerId != userId)
                return null;

            return MapToDto(fileItem);
        }

        /// <inheritdoc />
        public async Task<FileListDto> GetFileListAsync(FileListQuery query)
        {
            var (items, totalCount) = await _fileRepository.GetPagedAsync(
                query.OwnerId,
                query.ParentFolderId,
                query.PageIndex,
                query.PageSize,
                query.SortBy,
                query.Ascending);

            // 获取当前文件夹信息
            string? folderName = null;
            if (query.ParentFolderId.HasValue)
            {
                var folder = await _fileRepository.GetByIdAsync(query.ParentFolderId.Value);
                folderName = folder?.Name;
            }

            return new FileListDto
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                CurrentFolderId = query.ParentFolderId,
                CurrentFolderName = folderName
            };
        }

        /// <inheritdoc />
        public async Task<FileListDto> SearchFilesAsync(FileSearchQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.Keyword))
                return new FileListDto
                {
                    Items = [],
                    TotalCount = 0,
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize
                };

            var (items, totalCount) = await _fileRepository.SearchAsync(
                query.OwnerId, query.Keyword, query.PageIndex, query.PageSize);

            return new FileListDto
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }

        /// <inheritdoc />
        public async Task<FileInfoDto> CreateFolderAsync(CreateFolderCommand command)
        {
            var folder = FileItem.CreateFolder(command.FolderName, command.OwnerId, command.ParentFolderId);
            await _fileRepository.AddAsync(folder);

            return MapToDto(folder);
        }

        /// <inheritdoc />
        public async Task DeleteFileAsync(DeleteFileCommand command)
        {
            var fileItem = await _fileRepository.GetByIdAsync(command.FileId)
                ?? throw new InvalidOperationException("文件不存在");

            if (fileItem.OwnerId != command.UserId)
                throw new UnauthorizedAccessException("无权删除此文件");

            // 软删除
            fileItem.SoftDelete();
            await _fileRepository.UpdateAsync(fileItem);

            // 释放用户空间
            if (!fileItem.IsFolder)
            {
                var user = await _userRepository.GetByIdAsync(command.UserId);
                user!.DecreaseUsedSpace(fileItem.Size.bytesize);
                await _userRepository.UpdateAsync(user);
            }
        }

        /// <inheritdoc />
        public async Task<FileInfoDto> RenameFileAsync(RenameFileCommand command)
        {
            var fileItem = await _fileRepository.GetByIdAsync(command.FileId)
                ?? throw new InvalidOperationException("文件不存在");

            if (fileItem.OwnerId != command.UserId)
                throw new UnauthorizedAccessException("无权重命名此文件");

            fileItem.Rename(command.NewName);
            await _fileRepository.UpdateAsync(fileItem);

            return MapToDto(fileItem);
        }

        /// <inheritdoc />
        public async Task MoveFileAsync(MoveFileCommand command)
        {
            var fileItem = await _fileRepository.GetByIdAsync(command.FileId)
                ?? throw new InvalidOperationException("文件不存在");

            if (fileItem.OwnerId != command.UserId)
                throw new UnauthorizedAccessException("无权移动此文件");

            // 验证目标文件夹
            if (command.TargetFolderId.HasValue)
            {
                var targetFolder = await _fileRepository.GetByIdAsync(command.TargetFolderId.Value)
                    ?? throw new InvalidOperationException("目标文件夹不存在");

                if (!targetFolder.IsFolder)
                    throw new InvalidOperationException("目标不是文件夹");

                if (targetFolder.OwnerId != command.UserId)
                    throw new UnauthorizedAccessException("无权操作目标文件夹");
            }

            fileItem.MoveTo(command.TargetFolderId);
            await _fileRepository.UpdateAsync(fileItem);
        }

        /// <inheritdoc />
        public async Task<FileInfoDto> CopyFileAsync(Guid fileId, Guid userId, Guid? targetFolderId)
        {
            var fileItem = await _fileRepository.GetByIdAsync(fileId)
                ?? throw new InvalidOperationException("文件不存在");

            if (fileItem.OwnerId != userId)
                throw new UnauthorizedAccessException("无权复制此文件");

            if (fileItem.IsFolder)
                throw new InvalidOperationException("暂不支持复制文件夹");

            // 检查配额
            if (!await _quotaService.HasSufficientQuotaAsync(userId, fileItem.Size.bytesize))
                throw new InvalidOperationException("存储空间不足");

            // 创建副本（复用存储路径）
            var copy = FileItem.CreateFile(
                fileItem.Name,
                fileItem.Size,
                fileItem.StoragePath,
                fileItem.Hash,
                fileItem.MimeType,
                userId,
                targetFolderId);

            await _fileRepository.AddAsync(copy);

            // 更新已用空间
            var user = await _userRepository.GetByIdAsync(userId);
            user!.IncreaseUsedSpace(copy.Size.bytesize);
            await _userRepository.UpdateAsync(user);

            return MapToDto(copy);
        }

        /// <inheritdoc />
        public async Task<int> BatchDeleteAsync(BatchDeleteCommand command)
        {
            if (command.FileIds.Count == 0)
                return 0;

            var fileItems = await _fileRepository.GetByIdsAsync(command.FileIds);
            var ownedItems = fileItems.Where(f => f.OwnerId == command.UserId).ToList();

            if (ownedItems.Count == 0)
                return 0;

            long totalFreedBytes = 0;
            foreach (var item in ownedItems)
            {
                item.SoftDelete();
                if (!item.IsFolder)
                    totalFreedBytes += item.Size.bytesize;
            }

            await _fileRepository.UpdateRangeAsync(ownedItems);

            // 释放用户空间
            if (totalFreedBytes > 0)
            {
                var user = await _userRepository.GetByIdAsync(command.UserId);
                user!.DecreaseUsedSpace(totalFreedBytes);
                await _userRepository.UpdateAsync(user);
            }

            return ownedItems.Count;
        }

        /// <inheritdoc />
        public async Task<int> BatchMoveAsync(BatchMoveCommand command)
        {
            if (command.FileIds.Count == 0)
                return 0;

            // 验证目标文件夹
            if (command.TargetFolderId.HasValue)
            {
                var targetFolder = await _fileRepository.GetByIdAsync(command.TargetFolderId.Value)
                    ?? throw new InvalidOperationException("目标文件夹不存在");

                if (!targetFolder.IsFolder)
                    throw new InvalidOperationException("目标不是文件夹");

                if (targetFolder.OwnerId != command.UserId)
                    throw new UnauthorizedAccessException("无权操作目标文件夹");
            }

            var fileItems = await _fileRepository.GetByIdsAsync(command.FileIds);
            var ownedItems = fileItems.Where(f => f.OwnerId == command.UserId).ToList();

            if (ownedItems.Count == 0)
                return 0;

            FileItem.BatchMoveTo(ownedItems, command.TargetFolderId);
            await _fileRepository.UpdateRangeAsync(ownedItems);

            return ownedItems.Count;
        }

        /// <inheritdoc />
        public async Task<FileListDto> GetTrashListAsync(Guid userId, int pageIndex, int pageSize)
        {
            var (items, totalCount) = await _fileRepository.GetDeletedByOwnerAsync(userId, pageIndex, pageSize);

            return new FileListDto
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <inheritdoc />
        public async Task<FileInfoDto> RestoreFileAsync(RestoreFileCommand command)
        {
            var fileItem = await _fileRepository.GetDeletedByIdAsync(command.FileId)
                ?? throw new InvalidOperationException("文件不存在或未被删除");

            if (fileItem.OwnerId != command.UserId)
                throw new UnauthorizedAccessException("无权恢复此文件");

            // 检查配额（恢复文件需要重新占用空间）
            if (!fileItem.IsFolder)
            {
                if (!await _quotaService.HasSufficientQuotaAsync(command.UserId, fileItem.Size.bytesize))
                    throw new InvalidOperationException("存储空间不足，无法恢复文件");
            }

            fileItem.Restore();
            await _fileRepository.UpdateAsync(fileItem);

            // 恢复用户已用空间
            if (!fileItem.IsFolder)
            {
                var user = await _userRepository.GetByIdAsync(command.UserId);
                user!.IncreaseUsedSpace(fileItem.Size.bytesize);
                await _userRepository.UpdateAsync(user);
            }

            return MapToDto(fileItem);
        }

        /// <inheritdoc />
        public async Task<int> EmptyTrashAsync(Guid userId)
        {
            // 获取所有回收站文件（获取全部，不分页）
            var (items, _) = await _fileRepository.GetDeletedByOwnerAsync(userId, 1, int.MaxValue);

            if (items.Count == 0)
                return 0;

            // 永久删除
            foreach (var item in items)
            {
                await _fileRepository.DeleteAsync(item);
            }

            return items.Count;
        }

        /// <summary>
        /// 将FileItem实体映射为DTO
        /// </summary>
        private static FileInfoDto MapToDto(FileItem fileItem)
        {
            return new FileInfoDto
            {
                Id = fileItem.Id,
                Name = fileItem.Name,
                Extension = fileItem.Extension,
                Size = fileItem.Size.bytesize,
                FormattedSize = fileItem.GetFormattedSize(),
                MimeType = fileItem.MimeType,
                IsFolder = fileItem.IsFolder,
                ParentFolderId = fileItem.ParentFolderId,
                Hash = fileItem.Hash.hash,
                DownloadCount = fileItem.DownloadCount,
                Tags = fileItem.Tags,
                Description = fileItem.Description,
                ThumbnailUrl = fileItem.ThumbnailUrl,
                CreationTime = fileItem.CreationTime,
                LastModificationTime = fileItem.LastModificationTime
            };
        }
    }
}
