using Microsoft.Extensions.Logging;

namespace CloudDrive.Infrastructure.Services
{
    /// <summary>
    /// 通知服务实现（日志记录，可扩展为推送/WebSocket/邮件等）
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(Guid userId, string title, string message)
        {
            // 当前通过日志记录通知，后续可扩展为：
            // - WebSocket实时推送
            // - 邮件通知
            // - 短信通知
            // - 站内信存储
            _logger.LogInformation(
                "发送通知 - 用户ID：{UserId}，标题：{Title}，内容：{Message}",
                userId, title, message);

            return Task.CompletedTask;
        }

        public async Task SendAsync(IEnumerable<Guid> userIds, string title, string message)
        {
            foreach (var userId in userIds)
            {
                await SendAsync(userId, title, message);
            }
        }
    }
}
