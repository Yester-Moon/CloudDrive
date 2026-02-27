using CloudDrive.Domain.DomainEvents;
using CloudDrive.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudDrive.Infrastructure.EventHandlers
{
    /// <summary>
    /// 存储配额超限事件处理器 - 发送通知
    /// </summary>
    public class StorageQuotaExceededEventHandler : INotificationHandler<StorageQuotaExeededEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<StorageQuotaExceededEventHandler> _logger;

        public StorageQuotaExceededEventHandler(
            INotificationService notificationService,
            ILogger<StorageQuotaExceededEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(StorageQuotaExeededEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning(
                "存储配额超限 - 用户ID：{UserId}，已用：{UsedSpace}，配额：{TotalQuota}，尝试增加：{AttemptedSize}，缺少：{Shortfall}",
                notification.UserId,
                notification.CurrentUsedSpace,
                notification.TotalQuota,
                notification.AttemptedSize,
                notification.GetShortfall());

            // 发送通知
            await _notificationService.SendAsync(
                notification.UserId,
                "存储空间不足",
                $"您的存储空间已不足，当前已用 {FormatBytes(notification.CurrentUsedSpace)}，总配额 {FormatBytes(notification.TotalQuota)}。请清理文件或升级VIP。");
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = ["B", "KB", "MB", "GB", "TB"];
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
