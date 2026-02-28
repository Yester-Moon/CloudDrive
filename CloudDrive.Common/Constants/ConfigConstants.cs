namespace CloudDrive.Common.Constants
{
    /// <summary>
    /// 配置相关常量
    /// </summary>
    public static class ConfigConstants
    {
        #region 配置节名称

        /// <summary>
        /// JWT 配置节
        /// </summary>
        public const string JwtSection = "JWT";

        /// <summary>
        /// 存储配置节
        /// </summary>
        public const string StorageSection = "Storage";

        /// <summary>
        /// 邮件配置节
        /// </summary>
        public const string EmailSection = "Email";

        /// <summary>
        /// 数据库连接字符串名称
        /// </summary>
        public const string DefaultConnectionString = "DefaultConnection";

        #endregion

        #region 文件上传限制

        /// <summary>
        /// 默认单文件大小上限（10 GB）
        /// </summary>
        public const long DefaultMaxFileSize = 10L * 1024 * 1024 * 1024;

        /// <summary>
        /// 普通用户单文件大小上限（1 GB）
        /// </summary>
        public const long NormalUserMaxFileSize = 1L * 1024 * 1024 * 1024;

        /// <summary>
        /// VIP 用户单文件大小上限（5 GB）
        /// </summary>
        public const long VipUserMaxFileSize = 5L * 1024 * 1024 * 1024;

        #endregion

        #region 存储配额

        /// <summary>
        /// 普通用户默认存储配额（5 GB）
        /// </summary>
        public const long DefaultStorageQuota = 5L * 1024 * 1024 * 1024;

        /// <summary>
        /// VIP 用户存储配额（100 GB）
        /// </summary>
        public const long VipStorageQuota = 100L * 1024 * 1024 * 1024;

        /// <summary>
        /// SVIP 用户存储配额（2 TB）
        /// </summary>
        public const long SvipStorageQuota = 2L * 1024 * 1024 * 1024 * 1024;

        #endregion

        #region 分享设置

        /// <summary>
        /// 分享码长度
        /// </summary>
        public const int ShareCodeLength = 6;

        /// <summary>
        /// 分享链接默认过期天数
        /// </summary>
        public const int DefaultShareExpirationDays = 7;

        /// <summary>
        /// 分享链接最大过期天数
        /// </summary>
        public const int MaxShareExpirationDays = 365;

        #endregion

        #region 分页默认值

        /// <summary>
        /// 默认页码
        /// </summary>
        public const int DefaultPageIndex = 1;

        /// <summary>
        /// 默认每页数量
        /// </summary>
        public const int DefaultPageSize = 20;

        /// <summary>
        /// 最大每页数量
        /// </summary>
        public const int MaxPageSize = 100;

        #endregion

        #region 缓存

        /// <summary>
        /// 缓存默认过期时间（分钟）
        /// </summary>
        public const int DefaultCacheMinutes = 30;

        /// <summary>
        /// 用户信息缓存前缀
        /// </summary>
        public const string UserCachePrefix = "user:";

        /// <summary>
        /// 文件信息缓存前缀
        /// </summary>
        public const string FileCachePrefix = "file:";

        /// <summary>
        /// 分享信息缓存前缀
        /// </summary>
        public const string ShareCachePrefix = "share:";

        #endregion
    }
}
