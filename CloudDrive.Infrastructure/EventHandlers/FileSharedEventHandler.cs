using CloudDrive.Domain.DomainEvents;
using CloudDrive.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudDrive.Infrastructure.EventHandlers
{
    /// <summary>
    /// 文件分享事件处理器 — 记录日志并发送通知
    /// </summary>
    public class FileSharedEventHandler : INotificationHandler<FileSharedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<FileSharedEventHandler> _logger;

        public FileSharedEventHandler(
            INotificationService notificationService,
            ILogger<FileSharedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(FileSharedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "文件已分享 - 分享链接ID：{ShareLinkId}，文件ID：{FileId}，创建者ID：{CreatorId}，时间：{OccurredOn}",
                notification.ShareLinkId,
                notification.FileId,
                notification.CreatorId,
                notification.OccurredOn);

            await _notificationService.SendAsync(
                notification.CreatorId,
                "文件分享成功",
                $"您的文件分享链接已创建，分享链接ID：{notification.ShareLinkId}。");
        }
    }
}
