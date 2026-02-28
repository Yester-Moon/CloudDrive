using CloudDrive.Common.Interfaces;
using CloudDrive.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudDrive.Infrastructure.EventHandlers
{
    /// <summary>
    /// 配额变更事件处理器 — 失效配额缓存并记录日志
    /// </summary>
    public class QuotaChangedEventHandler : INotificationHandler<QuotaChangedEvent>
    {
        private readonly IMemoryCacheHelper _cacheHelper;
        private readonly ILogger<QuotaChangedEventHandler> _logger;

        public QuotaChangedEventHandler(
            IMemoryCacheHelper cacheHelper,
            ILogger<QuotaChangedEventHandler> logger)
        {
            _cacheHelper = cacheHelper;
            _logger = logger;
        }

        public Task Handle(QuotaChangedEvent notification, CancellationToken cancellationToken)
        {
            // 失效该用户的配额缓存，确保下次查询获取最新数据
            var cacheKey = $"user:quota:{notification.UserId}";
            _cacheHelper.Remove(cacheKey);

            _logger.LogInformation(
                "配额变更 - 用户ID：{UserId}，已用：{UsedSpace}，配额：{TotalQuota}，使用率：{UsagePercentage:F1}%，剩余：{Remaining}，时间：{OccurredOn}",
                notification.UserId,
                notification.CurrentUsedSpace,
                notification.TotalQuota,
                notification.GetUsagePercentage(),
                notification.GetRemainingSpace(),
                notification.OccurredOn);

            // 使用率超过 90% 时记录警告
            if (notification.GetUsagePercentage() >= 90)
            {
                _logger.LogWarning(
                    "用户存储空间即将用尽 - 用户ID：{UserId}，使用率：{UsagePercentage:F1}%",
                    notification.UserId,
                    notification.GetUsagePercentage());
            }

            return Task.CompletedTask;
        }
    }
}
