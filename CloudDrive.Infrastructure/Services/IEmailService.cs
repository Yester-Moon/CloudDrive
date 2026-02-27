namespace CloudDrive.Infrastructure.Services
{
    /// <summary>
    /// 邮件服务接口
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人地址</param>
        /// <param name="subject">主题</param>
        /// <param name="body">正文（HTML）</param>
        Task SendAsync(string to, string subject, string body);

        /// <summary>
        /// 发送邮件给多个收件人
        /// </summary>
        Task SendAsync(IEnumerable<string> to, string subject, string body);
    }
}
