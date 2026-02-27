namespace CloudDrive.Application.Dtos
{
    /// <summary>
    /// 文件信息DTO
    /// </summary>
    public class FileInfoDto
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 格式化的文件大小（如：1.5 MB）
        /// </summary>
        public string FormattedSize { get; set; } = string.Empty;

        /// <summary>
        /// MIME类型
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// 父文件夹ID
        /// </summary>
        public Guid? ParentFolderId { get; set; }

        /// <summary>
        /// 文件哈希
        /// </summary>
        public string Hash { get; set; } = string.Empty;

        /// <summary>
        /// 下载次数
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string? Tags { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 缩略图URL
        /// </summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }
    }
}
