using CloudDrive.Application.Services;
using CloudDrive.Domain.Entities;
using CloudDrive.Domain.Enums;
using CloudDrive.Domain.RepositoryInterfaces;
using CloudDrive.Domain.ValueObjects;

namespace CloudDrive.Application.Validators
{
    /// <summary>
    /// 文件上传业务规则验证服务
    /// </summary>
    public class FileUploadValidator
    {
        private readonly IFileRepository _fileRepository;
        private readonly IUserRepository _userRepository;
        private readonly FileDeduplicationService _deduplicationService;

        /// <summary>
        /// 禁止上传的文件扩展名（黑名单）
        /// </summary>
        private static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".exe", ".bat", ".cmd", ".com", ".scr",
            ".pif", ".vbs", ".js", ".wsf", ".msi"
        };

        public FileUploadValidator(
            IFileRepository fileRepository,
            IUserRepository userRepository,
            FileDeduplicationService deduplicationService)
        {
            _fileRepository = fileRepository;
            _userRepository = userRepository;
            _deduplicationService = deduplicationService;
        }

        /// <summary>
        /// 执行完整的文件上传验证
        /// </summary>
        /// <returns>验证结果</returns>
        public async Task<FileUploadValidationResult> ValidateAsync(
            Guid ownerId,
            string fileName,
            long fileSize,
            FileHash hash)
        {
            // 1. 验证文件名
            if (string.IsNullOrWhiteSpace(fileName))
                return FileUploadValidationResult.Fail("文件名不能为空");

            // 2. 验证文件类型（黑名单检查）
            var extension = Path.GetExtension(fileName);
            if (IsBlockedExtension(extension))
                return FileUploadValidationResult.Fail($"不允许上传 {extension} 类型的文件");

            // 3. 获取用户信息
            var user = await _userRepository.GetByIdAsync(ownerId);
            if (user == null)
                return FileUploadValidationResult.Fail("用户不存在");

            if (user.IsBanned)
                return FileUploadValidationResult.Fail("账户已被封禁，无法上传文件");

            // 4. 验证单文件大小限制（基于VIP等级）
            var vipLevel = user.GetEffectiveVipLevel();
            var maxFileSize = QuotaService.GetMaxSingleFileSize(vipLevel);
            if (fileSize > maxFileSize)
                return FileUploadValidationResult.Fail(
                    $"文件大小超出限制。当前VIP等级单文件最大：{FormatBytes(maxFileSize)}");

            // 5. 验证存储配额
            if (!user.HasEnoughSpace(fileSize))
                return FileUploadValidationResult.Fail(
                    $"存储空间不足。剩余：{FormatBytes(user.GetRemainingSpace())}，需要：{FormatBytes(fileSize)}");

            // 6. 去重检查
            var duplicate = await _deduplicationService.CheckDuplicateForOwnerAsync(hash, ownerId);
            if (duplicate != null)
                return FileUploadValidationResult.Duplicate(duplicate, "文件已存在（相同文件哈希）");

            // 7. 检查是否可以秒传（全局去重）
            var globalDuplicate = await _deduplicationService.CheckDuplicateGlobalAsync(hash);
            if (globalDuplicate != null)
                return FileUploadValidationResult.InstantUpload(globalDuplicate, "文件可秒传");

            return FileUploadValidationResult.Success();
        }

        /// <summary>
        /// 检查文件扩展名是否在黑名单中
        /// </summary>
        public static bool IsBlockedExtension(string extension)
        {
            return !string.IsNullOrEmpty(extension) && BlockedExtensions.Contains(extension);
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    /// <summary>
    /// 文件上传验证结果
    /// </summary>
    public class FileUploadValidationResult
    {
        /// <summary>
        /// 是否验证通过
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// 是否为重复文件（同一用户已存在）
        /// </summary>
        public bool IsDuplicate { get; private set; }

        /// <summary>
        /// 是否可秒传（全局已存在）
        /// </summary>
        public bool CanInstantUpload { get; private set; }

        /// <summary>
        /// 已存在的文件（重复或秒传引用）
        /// </summary>
        public FileItem? ExistingFile { get; private set; }

        /// <summary>
        /// 错误/提示消息
        /// </summary>
        public string? Message { get; private set; }

        public static FileUploadValidationResult Success()
            => new() { IsValid = true };

        public static FileUploadValidationResult Fail(string message)
            => new() { IsValid = false, Message = message };

        public static FileUploadValidationResult Duplicate(FileItem existingFile, string message)
            => new() { IsValid = false, IsDuplicate = true, ExistingFile = existingFile, Message = message };

        public static FileUploadValidationResult InstantUpload(FileItem existingFile, string message)
            => new() { IsValid = true, CanInstantUpload = true, ExistingFile = existingFile, Message = message };
    }
}
