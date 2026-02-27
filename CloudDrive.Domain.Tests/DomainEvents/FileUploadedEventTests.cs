using CloudDrive.Domain.DomainEvents;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CloudDrive.Domain.Tests.DomainEvents
{
    /// <summary>
    /// FileUploadedEvent 领域事件单元测试
    /// </summary>
    public class FileUploadedEventTests
    {
        [Fact]
        public void FileUploadedEvent_Should_Implement_INotification()
        {
            // Arrange & Act
            var fileUploadedEvent = new FileUploadedEvent(Guid.NewGuid(), Guid.NewGuid(), 1024);

            // Assert
            fileUploadedEvent.Should().BeAssignableTo<INotification>();
        }

        [Fact]
        public void FileUploadedEvent_Should_Create_With_Valid_Properties()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            long fileSize = 1024 * 1024; // 1MB
            var beforeCreate = DateTime.Now;

            // Act
            var fileUploadedEvent = new FileUploadedEvent(fileId, userId, fileSize);
            var afterCreate = DateTime.Now;

            // Assert
            fileUploadedEvent.FileId.Should().Be(fileId);
            fileUploadedEvent.UserId.Should().Be(userId);
            fileUploadedEvent.FileSize.Should().Be(fileSize);
            fileUploadedEvent.OccurredOn.Should().BeOnOrAfter(beforeCreate);
            fileUploadedEvent.OccurredOn.Should().BeOnOrBefore(afterCreate);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1024)]
        [InlineData(1024 * 1024)]
        [InlineData(1024L * 1024 * 1024 * 5)] // 5GB
        public void FileUploadedEvent_Should_Handle_Various_File_Sizes(long fileSize)
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act
            var fileUploadedEvent = new FileUploadedEvent(fileId, userId, fileSize);

            // Assert
            fileUploadedEvent.FileSize.Should().Be(fileSize);
        }

        [Fact]
        public void FileUploadedEvent_Should_Have_Different_OccurredOn_Times()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act
            var event1 = new FileUploadedEvent(fileId, userId, 1024);
            Thread.Sleep(10); // Small delay to ensure different timestamps
            var event2 = new FileUploadedEvent(fileId, userId, 1024);

            // Assert
            event2.OccurredOn.Should().BeAfter(event1.OccurredOn);
        }
    }
}
