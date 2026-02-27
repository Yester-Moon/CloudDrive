using CloudDrive.Domain.DomainEvents;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CloudDrive.Domain.Tests.DomainEvents
{
    /// <summary>
    /// FileSharedEvent 领域事件单元测试
    /// </summary>
    public class FileSharedEventTests
    {
        [Fact]
        public void FileSharedEvent_Should_Implement_INotification()
        {
            // Arrange & Act
            var fileSharedEvent = new FileSharedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            // Assert
            fileSharedEvent.Should().BeAssignableTo<INotification>();
        }

        [Fact]
        public void FileSharedEvent_Should_Create_With_Valid_Properties()
        {
            // Arrange
            var shareLinkId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var creatorId = Guid.NewGuid();
            var beforeCreate = DateTime.Now;

            // Act
            var fileSharedEvent = new FileSharedEvent(shareLinkId, fileId, creatorId);
            var afterCreate = DateTime.Now;

            // Assert
            fileSharedEvent.ShareLinkId.Should().Be(shareLinkId);
            fileSharedEvent.FileId.Should().Be(fileId);
            fileSharedEvent.CreatorId.Should().Be(creatorId);
            fileSharedEvent.OccurredOn.Should().BeOnOrAfter(beforeCreate);
            fileSharedEvent.OccurredOn.Should().BeOnOrBefore(afterCreate);
        }

        [Fact]
        public void FileSharedEvent_Should_Track_All_Related_Ids()
        {
            // Arrange
            var shareLinkId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var creatorId = Guid.NewGuid();

            // Act
            var fileSharedEvent = new FileSharedEvent(shareLinkId, fileId, creatorId);

            // Assert
            fileSharedEvent.ShareLinkId.Should().NotBeEmpty();
            fileSharedEvent.FileId.Should().NotBeEmpty();
            fileSharedEvent.CreatorId.Should().NotBeEmpty();
            fileSharedEvent.ShareLinkId.Should().NotBe(fileSharedEvent.FileId);
            fileSharedEvent.ShareLinkId.Should().NotBe(fileSharedEvent.CreatorId);
        }
    }
}
