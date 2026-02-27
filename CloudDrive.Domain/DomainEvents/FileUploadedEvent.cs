using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDrive.Domain.DomainEvents
{
    /// <summary>
    /// 文件上传完成事件
    /// </summary>
    public class FileUploadedEvent : INotification
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
        /// 文件大小（字节）
        /// </summary>
        public long FileSize { get; }

        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime OccurredOn { get; }

        public FileUploadedEvent(Guid fileId, Guid userId, long fileSize)
        {
            FileId = fileId;
            UserId = userId;
            FileSize = fileSize;
            OccurredOn = DateTime.Now;
        }
    }
}
