using CloudDrive.Domain.Entities;
using CloudDrive.Domain.RepositoryInterfaces;
using CloudDrive.Domain.ValueObjects;

namespace CloudDrive.Application.Services
{
    /// <summary>
    /// 文件去重领域服务
    /// </summary>
    public class FileDeduplicationService
    {
        private readonly IFileRepository _fileRepository;

        public FileDeduplicationService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        /// <summary>
        /// 检查当前用户是否已存在相同文件（同用户去重）
        /// </summary>
        /// <returns>已存在的文件项，若无重复则返回null</returns>
        public async Task<FileItem?> CheckDuplicateForOwnerAsync(FileHash hash, Guid ownerId)
        {
            if (string.IsNullOrWhiteSpace(hash.hash))
                return null;

            return await _fileRepository.GetByHashAndOwnerAsync(hash, ownerId);
        }

        /// <summary>
        /// 检查全局是否已存在相同文件（跨用户去重，秒传场景）
        /// </summary>
        /// <returns>已存在的文件项，若无重复则返回null</returns>
        public async Task<FileItem?> CheckDuplicateGlobalAsync(FileHash hash)
        {
            if (string.IsNullOrWhiteSpace(hash.hash))
                return null;

            return await _fileRepository.GetByHashAsync(hash);
        }

        /// <summary>
        /// 尝试秒传：如果全局存在相同哈希文件，为当前用户创建引用
        /// </summary>
        /// <returns>秒传成功返回新建的FileItem，否则返回null</returns>
        public async Task<FileItem?> TryInstantUploadAsync(
            FileHash hash,
            string fileName,
            string mimeType,
            Guid ownerId,
            Guid? parentFolderId = null)
        {
            // 先检查同一用户是否已有相同文件
            var ownerDuplicate = await CheckDuplicateForOwnerAsync(hash, ownerId);
            if (ownerDuplicate != null)
                return null; // 用户已有此文件，不重复创建

            // 查找全局是否存在相同哈希的文件
            var existingFile = await CheckDuplicateGlobalAsync(hash);
            if (existingFile == null)
                return null; // 无法秒传

            // 复用已有文件的存储路径，为当前用户创建新的文件记录
            var fileItem = FileItem.CreateFile(
                fileName,
                existingFile.Size,
                existingFile.StoragePath,
                hash,
                mimeType,
                ownerId,
                parentFolderId);

            return fileItem;
        }
    }
}
