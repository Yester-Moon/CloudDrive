namespace CloudDrive.Infrastructure.Storage
{
    /// <summary>
    /// 存储配置选项
    /// </summary>
    public class StorageOptions
    {
        public const string SectionName = "Storage";

        /// <summary>
        /// 存储提供者类型（Local / Oss）
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
}
