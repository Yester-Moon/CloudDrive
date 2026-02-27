using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloudDrive.Infrastructure.Services
{
    /// <summary>
    /// 邮件配置选项
    /// </summary>
    public class EmailOptions
    {
        public const string SectionName = "Email";

        /// <summary>
        /// SMTP服务器地址
        /// </summary>
        public string SmtpHost { get; set; } = "smtp.example.com";

        /// <summary>
        /// SMTP端口
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// 发件人地址
        /// </summary>
        public string FromAddress { get; set; } = "noreply@clouddrive.com";

        /// <summary>
        /// 发件人显示名称
        /// </summary>
        public string FromName { get; set; } = "CloudDrive";

        /// <summary>
        /// SMTP用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// SMTP密码
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用SSL
        /// </summary>
        public bool EnableSsl { get; set; } = true;
    }

    /// <summary>
    /// 邮件服务实现（SMTP）
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _options;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            await SendAsync([to], subject, body);
        }

        public async Task SendAsync(IEnumerable<string> to, string subject, string body)
        {
            try
            {
                using var client = new System.Net.Mail.SmtpClient(_options.SmtpHost, _options.SmtpPort)
                {
                    Credentials = new System.Net.NetworkCredential(_options.UserName, _options.Password),
                    EnableSsl = _options.EnableSsl
                };

                var message = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(_options.FromAddress, _options.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                foreach (var address in to)
                {
                    message.To.Add(address);
                }

                await client.SendMailAsync(message);

                _logger.LogInformation("邮件发送成功 - 收件人：{To}，主题：{Subject}",
                    string.Join(", ", to), subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "邮件发送失败 - 收件人：{To}，主题：{Subject}",
                    string.Join(", ", to), subject);
                throw;
            }
        }
    }
}
