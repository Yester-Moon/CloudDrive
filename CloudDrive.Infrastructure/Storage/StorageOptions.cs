namespace CloudDrive.Infrastructure.Storage
{
    /// <summary>
    /// 存储配置选项
    /// </summary>
    public class StorageOptions
    {
        public const string SectionName = "Storage";

        /// <summary>
        /// 存储提供者类型（Local / Oss / Hybrid）
        /// </summary>
        public string Provider { get; set; } = "Local";

        /// <summary>
        /// 本地存储根路径
        /// </summary>
        public string LocalStoragePath { get; set; } = "uploads";

        /// <summary>
        /// OSS配置
        /// </summary>
        public OssOptions Oss { get; set; } = new();

        /// <summary>
        /// 混合存储配置
        /// </summary>
        public HybridStorageOptions Hybrid { get; set; } = new();
    }

    /// <summary>
    /// OSS配置选项
    /// </summary>
    public class OssOptions
    {
        /// <summary>
        /// Endpoint（如：oss-cn-hangzhou.aliyuncs.com）
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// AccessKeyId
        /// </summary>
        public string AccessKeyId { get; set; } = string.Empty;

        /// <summary>
        /// AccessKeySecret
        /// </summary>
        public string AccessKeySecret { get; set; } = string.Empty;

        /// <summary>
        /// Bucket名称
        /// </summary>
        public string BucketName { get; set; } = string.Empty;

        /// <summary>
        /// 自定义域名（可选）
        /// </summary>
        public string? CustomDomain { get; set; }
    }

    /// <summary>
    /// 混合存储（热/冷分层）配置
    /// </summary>
    public class HybridStorageOptions
    {
        /// <summary>
        /// 热数据存储层（默认 Local）
        /// </summary>
        public string HotTier { get; set; } = "Local";

        /// <summary>
        /// 冷数据存储层（默认 Oss）
        /// </summary>
        public string ColdTier { get; set; } = "Oss";

        /// <summary>
        /// 热/冷分界阈值：文件大小（字节），超过此值存入冷层
        /// 默认 50MB
        /// </summary>
        public long ColdThresholdBytes { get; set; } = 50L * 1024 * 1024;
    }

    /// <summary>
    /// 存储层级枚举
    /// </summary>
    public enum StorageTier
    {
        /// <summary>
        /// 热数据 — 高频访问，低延迟（如本地 SSD）
        /// </summary>
        Hot,

        /// <summary>
        /// 冷数据 — 低频访问，低成本（如 OSS / S3）
        /// </summary>
        Cold
    }
}
