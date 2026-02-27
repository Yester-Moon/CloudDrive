namespace CloudDrive.Application.Commands
{
    /// <summary>
    /// 创建分享链接命令
    /// </summary>
    public class CreateShareLinkCommand
    {
        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid CreatorId { get; set; }

        /// <summary>
        /// 被分享的文件ID
        /// </summary>
        public Guid FileItemId { get; set; }

        /// <summary>
        /// 分享标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 访问密码（null表示无密码）
        /// </summary>
        public string? AccessPassword { get; set; }

        /// <summary>
        /// 过期时间（null表示永久有效）
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// 最大下载次数（null表示无限制）
        /// </summary>
        public int? MaxDownloadCount { get; set; }

        /// <summary>
        /// 是否允许下载
        /// </summary>
        public bool AllowDownload { get; set; } = true;
    }

    /// <summary>
    /// 取消分享命令
    /// </summary>
    public class CancelShareCommand
    {
        /// <summary>
        /// 分享链接ID
        /// </summary>
        public Guid ShareLinkId { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
