using MediatR;
using System;

namespace CloudDrive.Domain.DomainEvents
{
    /// <summary>
    /// 配额变更事件
    /// </summary>
    public class QuotaChangedEvent : INotification
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// 当前已使用空间（字节）
        /// </summary>
        public long CurrentUsedSpace { get; }

        /// <summary>
        /// 总配额（字节）
        /// </summary>
        public long TotalQuota { get; }

        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime OccurredOn { get; }

        public QuotaChangedEvent(Guid userId, long currentUsedSpace, long totalQuota)
        {
            UserId = userId;
            CurrentUsedSpace = currentUsedSpace;
            TotalQuota = totalQuota;
            OccurredOn = DateTime.Now;
        }

        /// <summary>
        /// 获取剩余空间
        /// </summary>
        public long GetRemainingSpace()
        {
            return Math.Max(0, TotalQuota - CurrentUsedSpace);
        }

        /// <summary>
        /// 获取使用率（百分比）
        /// </summary>
        public double GetUsagePercentage()
        {
            if (TotalQuota == 0) return 0;
            return (double)CurrentUsedSpace / TotalQuota * 100;
        }
    }
}
