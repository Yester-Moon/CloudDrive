namespace CloudDrive.Application.Dtos
{
    /// <summary>
    /// 文件上传结果DTO
    /// </summary>
    public class FileUploadResultDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid? FileId { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 是否为秒传
        /// </summary>
        public bool IsInstantUpload { get; set; }

        /// <summary>
        /// 是否为重复文件
        /// </summary>
        public bool IsDuplicate { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }

        public static FileUploadResultDto Ok(Guid fileId, string fileName, long fileSize, bool isInstantUpload = false)
            => new()
            {
                Success = true,
                FileId = fileId,
                FileName = fileName,
                FileSize = fileSize,
                IsInstantUpload = isInstantUpload
            };

        public static FileUploadResultDto Fail(string errorMessage)
            => new() { Success = false, ErrorMessage = errorMessage };

        public static FileUploadResultDto DuplicateFile(Guid existingFileId, string fileName)
            => new()
            {
                Success = false,
                IsDuplicate = true,
                FileId = existingFileId,
                FileName = fileName,
                ErrorMessage = "文件已存在"
            };
    }
}
