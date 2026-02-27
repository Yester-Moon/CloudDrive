using CloudDrive.Application.Commands;
using CloudDrive.Application.Dtos;
using CloudDrive.Application.Interfaces;
using CloudDrive.Domain.Entities;
using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.RepositoryInterfaces;

namespace CloudDrive.Application.Services
{
    /// <summary>
    /// 分享应用服务
    /// </summary>
    public class ShareService : IShareService
    {
        private readonly IShareLinkRepository _shareLinkRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IStorageProvider _storageProvider;
        private readonly ShareLinkAccessService _accessService;

        public ShareService(
            IShareLinkRepository shareLinkRepository,
            IFileRepository fileRepository,
            IStorageProvider storageProvider,
            ShareLinkAccessService accessService)
        {
            _shareLinkRepository = shareLinkRepository;
            _fileRepository = fileRepository;
            _storageProvider = storageProvider;
            _accessService = accessService;
        }

        /// <inheritdoc />
        public async Task<ShareLinkDto> CreateShareLinkAsync(CreateShareLinkCommand command)
        {
            // 验证文件
            var fileItem = await _fileRepository.GetByIdAsync(command.FileItemId)
                ?? throw new InvalidOperationException("文件不存在");

            // 验证创建分享权限
            var validation = _accessService.ValidateCreateShare(fileItem, command.CreatorId);
            if (!validation.IsAllowed)
                throw new InvalidOperationException(validation.Message);

            // 创建分享链接
            var shareLink = ShareLink.Create(
                command.FileItemId,
                command.CreatorId,
                command.Title,
                command.AccessPassword,
                command.ExpirationTime,
                command.MaxDownloadCount,
                command.AllowDownload);

            await _shareLinkRepository.AddAsync(shareLink);

            return MapToDto(shareLink, fileItem);
        }

        /// <inheritdoc />
        public async Task<ShareLinkDto?> GetShareByCodeAsync(string shareCode)
        {
            var shareLink = await _shareLinkRepository.GetByShareCodeAsync(shareCode);
            if (shareLink == null) return null;

            var fileItem = await _fileRepository.GetByIdAsync(shareLink.FileItemId);

            // 增加访问次数
            shareLink.IncrementViewCount();
            await _shareLinkRepository.UpdateAsync(shareLink);

            return MapToDto(shareLink, fileItem);
        }

        /// <inheritdoc />
        public async Task<FileInfoDto?> GetSharedFileAsync(string shareCode, string? password = null)
        {
            var shareLink = await _shareLinkRepository.GetByShareCodeAsync(shareCode)
                ?? throw new InvalidOperationException("分享链接不存在");

            // 验证访问权限
            var validation = _accessService.ValidateAccess(shareLink, password);
            if (!validation.IsAllowed)
                throw new InvalidOperationException(validation.Message);

            var fileItem = await _fileRepository.GetByIdAsync(shareLink.FileItemId);
            if (fileItem == null) return null;

            return new FileInfoDto
            {
                Id = fileItem.Id,
                Name = fileItem.Name,
                Extension = fileItem.Extension,
                Size = fileItem.Size.bytesize,
                FormattedSize = fileItem.GetFormattedSize(),
                MimeType = fileItem.MimeType,
                IsFolder = fileItem.IsFolder,
                Hash = fileItem.Hash.hash,
                DownloadCount = fileItem.DownloadCount,
                ThumbnailUrl = fileItem.ThumbnailUrl,
                CreationTime = fileItem.CreationTime,
                LastModificationTime = fileItem.LastModificationTime
            };
        }

        /// <inheritdoc />
        public async Task<(Stream FileStream, string FileName, string MimeType)> DownloadSharedFileAsync(
            string shareCode, string? password = null)
        {
            var shareLink = await _shareLinkRepository.GetByShareCodeAsync(shareCode)
                ?? throw new InvalidOperationException("分享链接不存在");

            // 验证下载权限
            var validation = _accessService.ValidateDownload(shareLink, password);
            if (!validation.IsAllowed)
                throw new InvalidOperationException(validation.Message);

            var fileItem = await _fileRepository.GetByIdAsync(shareLink.FileItemId)
                ?? throw new InvalidOperationException("分享的文件已被删除");

            // 增加下载次数
            shareLink.IncrementDownloadCount();
            await _shareLinkRepository.UpdateAsync(shareLink);

            fileItem.IncrementDownloadCount();
            await _fileRepository.UpdateAsync(fileItem);

            var stream = await _storageProvider.DownloadAsync(fileItem.StoragePath);

            return (stream, fileItem.Name, fileItem.MimeType);
        }

        /// <inheritdoc />
        public async Task<List<ShareLinkDto>> GetUserSharesAsync(Guid userId)
        {
            var shareLinks = await _shareLinkRepository.GetByCreatorIdAsync(userId);
            var result = new List<ShareLinkDto>();

            foreach (var shareLink in shareLinks)
            {
                var fileItem = await _fileRepository.GetByIdAsync(shareLink.FileItemId);
                result.Add(MapToDto(shareLink, fileItem));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task CancelShareAsync(CancelShareCommand command)
        {
            var shareLink = await _shareLinkRepository.GetByIdAsync(command.ShareLinkId)
                ?? throw new InvalidOperationException("分享链接不存在");

            // 验证所有者权限
            var validation = _accessService.ValidateOwnership(shareLink, command.UserId);
            if (!validation.IsAllowed)
                throw new UnauthorizedAccessException(validation.Message);

            shareLink.Cancel();
            await _shareLinkRepository.UpdateAsync(shareLink);
        }

        /// <inheritdoc />
        public async Task<ShareLinkDto?> GetShareStatsAsync(Guid shareLinkId, Guid userId)
        {
            var shareLink = await _shareLinkRepository.GetByIdAsync(shareLinkId);
            if (shareLink == null || shareLink.CreatorId != userId)
                return null;

            var fileItem = await _fileRepository.GetByIdAsync(shareLink.FileItemId);
            return MapToDto(shareLink, fileItem);
        }

        private static ShareLinkDto MapToDto(ShareLink shareLink, FileItem? fileItem)
        {
            return new ShareLinkDto
            {
                Id = shareLink.Id,
                ShareCode = shareLink.ShareCode,
                Title = shareLink.Title,
                FileItemId = shareLink.FileItemId,
                FileName = fileItem?.Name,
                FileSize = fileItem?.Size.bytesize ?? 0,
                HasPassword = !string.IsNullOrEmpty(shareLink.AccessPassword),
                ExpirationTime = shareLink.ExpirationTime,
                MaxDownloadCount = shareLink.MaxDownloadCount,
                CurrentDownloadCount = shareLink.CurrentDownloadCount,
                RemainingDownloadCount = shareLink.GetRemainingDownloadCount(),
                ViewCount = shareLink.ViewCount,
                AllowDownload = shareLink.AllowDownload,
                Status = shareLink.GetStatusDescription(),
                IsValid = shareLink.IsValid(),
                CreationTime = shareLink.CreationTime
            };
        }
    }
}
