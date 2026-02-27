using CloudDrive.Domain.DomainEvents;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CloudDrive.Domain.Tests.DomainEvents
{
    /// <summary>
    /// FileDeletedEvent 领域事件单元测试
    /// </summary>
    public class FileDeletedEventTests
    {
        [Fact]
        public void FileDeletedEvent_Should_Implement_INotification()
        {
            // Arrange & Act
            var fileDeletedEvent = new FileDeletedEvent(Guid.NewGuid(), Guid.NewGuid(), 1024);

            // Assert
            fileDeletedEvent.Should().BeAssignableTo<INotification>();
        }

        [Fact]
        public void FileDeletedEvent_Should_Create_With_Valid_Properties()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            long fileSize = 2048;
            var beforeCreate = DateTime.Now;

            // Act
            var fileDeletedEvent = new FileDeletedEvent(fileId, userId, fileSize);
            var afterCreate = DateTime.Now;

            // Assert
            fileDeletedEvent.FileId.Should().Be(fileId);
            fileDeletedEvent.UserId.Should().Be(userId);
            fileDeletedEvent.FileSize.Should().Be(fileSize);
            fileDeletedEvent.OccurredOn.Should().BeOnOrAfter(beforeCreate);
            fileDeletedEvent.OccurredOn.Should().BeOnOrBefore(afterCreate);
        }

        [Fact]
        public void FileDeletedEvent_Should_Track_FileSize_For_Quota_Release()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            long fileSize = 1024 * 1024 * 100; // 100MB

            // Act
            var fileDeletedEvent = new FileDeletedEvent(fileId, userId, fileSize);

            // Assert
            fileDeletedEvent.FileSize.Should().Be(fileSize);
        }
    }
}
