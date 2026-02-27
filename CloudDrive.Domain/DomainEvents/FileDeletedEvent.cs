using MediatR;
using System;

namespace CloudDrive.Domain.DomainEvents
{
    /// <summary>
    /// 文件删除事件
    /// </summary>
    public class FileDeletedEvent : INotification
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// 文件大小（字节，用于释放配额）
        /// </summary>
        public long FileSize { get; }

        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime OccurredOn { get; }

        public FileDeletedEvent(Guid fileId, Guid userId, long fileSize)
        {
            FileId = fileId;
            UserId = userId;
            FileSize = fileSize;
            OccurredOn = DateTime.Now;
        }
    }
}
