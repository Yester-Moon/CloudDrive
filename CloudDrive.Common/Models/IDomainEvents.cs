using MediatR;

namespace CloudDrive.Common.Models
{
    public interface IDomainEvents
    {
        IEnumerable<INotification> GetNotifications();

        void ClearNotifications();

        void AddNotification(INotification notification);
    }
}