using CloudDrive.Common.Constants;

namespace CloudDrive.Common.Exceptions
{
    /// <summary>
    /// 存储配额超限异常
    /// </summary>
    public class QuotaExceededException : BusinessException
    {
        /// <summary>
        /// 已使用空间（字节）
        /// </summary>
        public long UsedSpace { get; }

        /// <summary>
        /// 配额上限（字节）
        /// </summary>
        public long TotalQuota { get; }

        /// <summary>
        /// 本次需要的空间（字节）
        /// </summary>
        public long RequiredSpace { get; }

        public QuotaExceededException(long usedSpace, long totalQuota, long requiredSpace)
            : base($"存储空间不足。已使用：{usedSpace}，总配额：{totalQuota}，需要：{requiredSpace}", ErrorCodes.QuotaExceeded)
        {
            UsedSpace = usedSpace;
            TotalQuota = totalQuota;
            RequiredSpace = requiredSpace;
        }

        public QuotaExceededException(string message)
            : base(message, ErrorCodes.QuotaExceeded)
        {
        }
    }

    /// <summary>
    /// 文件不存在异常
    /// </summary>
    public class FileNotExistException : BusinessException
    {
        /// <summary>
        /// 文件 ID
        /// </summary>
        public Guid? FileId { get; }

        public FileNotExistException(Guid fileId)
            : base($"文件不存在（ID: {fileId}）", ErrorCodes.FileNotFound)
        {
            FileId = fileId;
        }

        public FileNotExistException(string message)
            : base(message, ErrorCodes.FileNotFound)
        {
        }
    }

    /// <summary>
    /// 文件类型不允许异常
    /// </summary>
    public class FileTypeNotAllowedException : BusinessException
    {
        /// <summary>
        /// 被拒绝的文件扩展名
        /// </summary>
        public string Extension { get; }

        public FileTypeNotAllowedException(string extension)
            : base($"不允许上传 {extension} 类型的文件", ErrorCodes.FileTypeNotAllowed)
        {
            Extension = extension;
        }
    }

    /// <summary>
    /// 文件大小超限异常
    /// </summary>
    public class FileSizeExceededException : BusinessException
    {
        /// <summary>
        /// 文件实际大小（字节）
        /// </summary>
        public long ActualSize { get; }

        /// <summary>
        /// 允许的最大大小（字节）
        /// </summary>
        public long MaxAllowedSize { get; }

        public FileSizeExceededException(long actualSize, long maxAllowedSize)
            : base($"文件大小超出限制。实际：{actualSize}，最大允许：{maxAllowedSize}", ErrorCodes.FileSizeExceeded)
        {
            ActualSize = actualSize;
            MaxAllowedSize = maxAllowedSize;
        }
    }

    /// <summary>
    /// 用户不存在异常
    /// </summary>
    public class UserNotFoundException : BusinessException
    {
        public Guid? UserId { get; }

        public UserNotFoundException(Guid userId)
            : base($"用户不存在（ID: {userId}）", ErrorCodes.UserNotFound)
        {
            UserId = userId;
        }

        public UserNotFoundException(string message)
            : base(message, ErrorCodes.UserNotFound)
        {
        }
    }

    /// <summary>
    /// 用户名或密码错误异常
    /// </summary>
    public class InvalidCredentialsException : BusinessException
    {
        public InvalidCredentialsException()
            : base("用户名或密码错误", ErrorCodes.InvalidCredentials)
        {
        }

        public InvalidCredentialsException(string message)
            : base(message, ErrorCodes.InvalidCredentials)
        {
        }
    }

    /// <summary>
    /// 用户已被封禁异常
    /// </summary>
    public class UserBannedException : BusinessException
    {
        public UserBannedException()
            : base("账户已被封禁", ErrorCodes.UserBanned)
        {
        }

        public UserBannedException(string message)
            : base(message, ErrorCodes.UserBanned)
        {
        }
    }

    /// <summary>
    /// 重复操作异常（如重复上传、重复注册等）
    /// </summary>
    public class DuplicateException : BusinessException
    {
        public DuplicateException(string message)
            : base(message, ErrorCodes.DuplicateResource)
        {
        }
    }

    /// <summary>
    /// 权限不足异常
    /// </summary>
    public class ForbiddenException : BusinessException
    {
        public ForbiddenException()
            : base("权限不足", ErrorCodes.Forbidden)
        {
        }

        public ForbiddenException(string message)
            : base(message, ErrorCodes.Forbidden)
        {
        }
    }

    /// <summary>
    /// 分享链接无效异常
    /// </summary>
    public class ShareLinkInvalidException : BusinessException
    {
        public ShareLinkInvalidException(string message)
            : base(message, ErrorCodes.ShareLinkInvalid)
        {
        }

        public ShareLinkInvalidException()
            : base("分享链接已失效", ErrorCodes.ShareLinkInvalid)
        {
        }
    }

    /// <summary>
    /// 分享密码错误异常
    /// </summary>
    public class SharePasswordIncorrectException : BusinessException
    {
        public SharePasswordIncorrectException()
            : base("分享密码错误", ErrorCodes.SharePasswordIncorrect)
        {
        }
    }
}
