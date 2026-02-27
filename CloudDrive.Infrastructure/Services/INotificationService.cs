namespace CloudDrive.Infrastructure.Services
{
    /// <summary>
    /// 通知服务接口
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// 发送通知给指定用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="title">通知标题</param>
        /// <param name="message">通知内容</param>
        Task SendAsync(Guid userId, string title, string message);

        /// <summary>
        /// 发送通知给多个用户
        /// </summary>
        Task SendAsync(IEnumerable<Guid> userIds, string title, string message);
    }
}
