using CloudDrive.Domain.DomainEvents;
using CloudDrive.Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudDrive.Infrastructure.EventHandlers
{
    /// <summary>
    /// 文件上传事件处理器 - 记录日志
    /// </summary>
    public class FileUploadedEventHandler : INotificationHandler<FileUploadedEvent>
    {
        private readonly ILogger<FileUploadedEventHandler> _logger;

        public FileUploadedEventHandler(ILogger<FileUploadedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(FileUploadedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "文件上传完成 - 文件ID：{FileId}，用户ID：{UserId}，大小：{FileSize} 字节，时间：{OccurredOn}",
                notification.FileId,
                notification.UserId,
                notification.FileSize,
                notification.OccurredOn);

            return Task.CompletedTask;
        }
    }
}
