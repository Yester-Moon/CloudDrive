using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDrive.Domain.DomainEvents
{
    /// <summary>
    /// 存储配额超限事件
    /// </summary>
    public class StorageQuotaExeededEvent : INotification
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
        /// 尝试增加的大小（字节）
        /// </summary>
        public long AttemptedSize { get; }

        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime OccurredOn { get; }

        public StorageQuotaExeededEvent(Guid userId, long currentUsedSpace, long totalQuota, long attemptedSize)
        {
            UserId = userId;
            CurrentUsedSpace = currentUsedSpace;
            TotalQuota = totalQuota;
            AttemptedSize = attemptedSize;
            OccurredOn = DateTime.Now;
        }

        /// <summary>
        /// 获取缺少的空间大小
        /// </summary>
        public long GetShortfall()
        {
            return CurrentUsedSpace + AttemptedSize - TotalQuota;
        }
    }
}
