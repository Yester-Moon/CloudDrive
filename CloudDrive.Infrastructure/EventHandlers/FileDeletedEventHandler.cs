using CloudDrive.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudDrive.Infrastructure.EventHandlers
{
    /// <summary>
    /// 文件删除事件处理器 - 记录日志
    /// </summary>
    public class FileDeletedEventHandler : INotificationHandler<FileDeletedEvent>
    {
        private readonly ILogger<FileDeletedEventHandler> _logger;

        public FileDeletedEventHandler(ILogger<FileDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(FileDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "文件已删除 - 文件ID：{FileId}，用户ID：{UserId}，释放空间：{FileSize} 字节，时间：{OccurredOn}",
                notification.FileId,
                notification.UserId,
                notification.FileSize,
                notification.OccurredOn);

            return Task.CompletedTask;
        }
    }
}
