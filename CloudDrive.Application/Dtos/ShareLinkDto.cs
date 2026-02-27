namespace CloudDrive.Application.Dtos
{
    /// <summary>
    /// 分享链接DTO
    /// </summary>
    public class ShareLinkDto
    {
        /// <summary>
        /// 分享链接ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 分享码
        /// </summary>
        public string ShareCode { get; set; } = string.Empty;

        /// <summary>
        /// 分享标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 被分享的文件ID
        /// </summary>
        public Guid FileItemId { get; set; }

        /// <summary>
        /// 被分享的文件名
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// 被分享的文件大小（字节）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 是否有访问密码
        /// </summary>
        public bool HasPassword { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// 最大下载次数
        /// </summary>
        public int? MaxDownloadCount { get; set; }

        /// <summary>
        /// 当前下载次数
        /// </summary>
        public int CurrentDownloadCount { get; set; }

        /// <summary>
        /// 剩余下载次数
        /// </summary>
        public int? RemainingDownloadCount { get; set; }

        /// <summary>
        /// 访问次数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 是否允许下载
        /// </summary>
        public bool AllowDownload { get; set; }

        /// <summary>
        /// 状态描述
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
