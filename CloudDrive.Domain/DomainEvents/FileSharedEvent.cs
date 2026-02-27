using MediatR;
using System;

namespace CloudDrive.Domain.DomainEvents
{
    /// <summary>
    /// 文件分享事件
    /// </summary>
    public class FileSharedEvent : INotification
    {
        /// <summary>
        /// 分享链接ID
        /// </summary>
        public Guid ShareLinkId { get; }

        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid CreatorId { get; }

        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime OccurredOn { get; }

        public FileSharedEvent(Guid shareLinkId, Guid fileId, Guid creatorId)
        {
            ShareLinkId = shareLinkId;
            FileId = fileId;
            CreatorId = creatorId;
            OccurredOn = DateTime.Now;
        }
    }
}
