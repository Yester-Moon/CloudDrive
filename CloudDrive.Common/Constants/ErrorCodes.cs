namespace CloudDrive.Common.Constants
{
    /// <summary>
    /// 业务错误码常量
    /// </summary>
    public static class ErrorCodes
    {
        #region 通用错误 (400xx)

        /// <summary>
        /// 请求参数无效
        /// </summary>
        public const int InvalidParameter = 40001;

        /// <summary>
        /// 资源重复
        /// </summary>
        public const int DuplicateResource = 40002;

        /// <summary>
        /// 操作不允许
        /// </summary>
        public const int OperationNotAllowed = 40003;

        #endregion

        #region 认证与授权 (401xx / 403xx)

        /// <summary>
        /// 未认证
        /// </summary>
        public const int Unauthorized = 40100;

        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        public const int InvalidCredentials = 40101;

        /// <summary>
        /// Token 已过期
        /// </summary>
        public const int TokenExpired = 40102;

        /// <summary>
        /// 权限不足
        /// </summary>
        public const int Forbidden = 40300;

        /// <summary>
        /// 账户已被封禁
        /// </summary>
        public const int UserBanned = 40301;

        #endregion

        #region 资源不存在 (404xx)

        /// <summary>
        /// 文件不存在
        /// </summary>
        public const int FileNotFound = 40401;

        /// <summary>
        /// 文件夹不存在
        /// </summary>
        public const int FolderNotFound = 40402;

        /// <summary>
        /// 用户不存在
        /// </summary>
        public const int UserNotFound = 40403;

        /// <summary>
        /// 分享链接不存在
        /// </summary>
        public const int ShareLinkNotFound = 40404;

        #endregion

        #region 文件相关 (410xx)

        /// <summary>
        /// 文件类型不允许
        /// </summary>
        public const int FileTypeNotAllowed = 41001;

        /// <summary>
        /// 文件大小超限
        /// </summary>
        public const int FileSizeExceeded = 41002;

        /// <summary>
        /// 存储配额超限
        /// </summary>
        public const int QuotaExceeded = 41003;

        /// <summary>
        /// 文件哈希不匹配
        /// </summary>
        public const int FileHashMismatch = 41004;

        /// <summary>
        /// 文件上传失败
        /// </summary>
        public const int FileUploadFailed = 41005;

        #endregion

        #region 分享相关 (420xx)

        /// <summary>
        /// 分享链接已失效
        /// </summary>
        public const int ShareLinkInvalid = 42001;

        /// <summary>
        /// 分享密码错误
        /// </summary>
        public const int SharePasswordIncorrect = 42002;

        /// <summary>
        /// 分享下载次数已达上限
        /// </summary>
        public const int ShareDownloadLimitReached = 42003;

        #endregion

        #region 服务器错误 (500xx)

        /// <summary>
        /// 内部服务器错误
        /// </summary>
        public const int InternalError = 50000;

        /// <summary>
        /// 存储服务异常
        /// </summary>
        public const int StorageServiceError = 50001;

        /// <summary>
        /// 邮件服务异常
        /// </summary>
        public const int EmailServiceError = 50002;

        #endregion
    }
}
