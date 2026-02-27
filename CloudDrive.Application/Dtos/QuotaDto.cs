namespace CloudDrive.Application.Dtos
{
    /// <summary>
    /// 配额信息DTO
    /// </summary>
    public class QuotaDto
    {
        /// <summary>
        /// 总配额（字节）
        /// </summary>
        public long TotalQuota { get; set; }

        /// <summary>
        /// 已使用空间（字节）
        /// </summary>
        public long UsedSpace { get; set; }

        /// <summary>
        /// 剩余空间（字节）
        /// </summary>
        public long RemainingSpace { get; set; }

        /// <summary>
        /// 使用率（百分比）
        /// </summary>
        public double UsagePercentage { get; set; }

        /// <summary>
        /// 格式化的总配额
        /// </summary>
        public string FormattedTotalQuota { get; set; } = string.Empty;

        /// <summary>
        /// 格式化的已用空间
        /// </summary>
        public string FormattedUsedSpace { get; set; } = string.Empty;

        /// <summary>
        /// 格式化的剩余空间
        /// </summary>
        public string FormattedRemainingSpace { get; set; } = string.Empty;

        /// <summary>
        /// VIP等级
        /// </summary>
        public int VipLevel { get; set; }

        /// <summary>
        /// 单文件最大大小（字节）
        /// </summary>
        public long MaxSingleFileSize { get; set; }
    }
}
