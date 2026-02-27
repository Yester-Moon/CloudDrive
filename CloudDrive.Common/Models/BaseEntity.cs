using MediatR;

namespace CloudDrive.Common.Models
{
    public record class BaseEntity : IEntity, IDomainEvents
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        private readonly List<INotification> _domainEvents = new List<INotification>();
        public void AddNotification(INotification notification)
        {
            if (!_domainEvents.Contains(notification)) _domainEvents.Add(notification);
        }

        public void ClearNotifications()
        {
            _domainEvents.Clear();
        }

        public IEnumerable<INotification> GetNotifications()
        {
            return _domainEvents;
        }
    }
}