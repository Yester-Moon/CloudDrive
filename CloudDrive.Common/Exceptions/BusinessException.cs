namespace CloudDrive.Common.Exceptions
{
    /// <summary>
    /// 业务异常基类
    /// 所有可预见的业务规则违反应抛出此异常或其子类
    /// </summary>
    public class BusinessException : Exception
    {
        /// <summary>
        /// 业务错误码
        /// </summary>
        public int ErrorCode { get; }

        public BusinessException(string message, int errorCode = 400)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public BusinessException(string message, int errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
