using CloudDrive.Common.Models;
using CloudDrive.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CloudDrive.Domain.Tests.Entities
{
    /// <summary>
    /// ShareLink 实体单元测试
    /// </summary>
    public class ShareLinkTests
    {
        [Fact]
        public void ShareLink_Should_Inherit_From_AggregateRootEntity()
        {
            // Arrange
            var shareLink = CreateTestShareLink();

            // Assert
            shareLink.Should().BeAssignableTo<AggregateRootEntity>();
            shareLink.Should().BeAssignableTo<IAggregateRoot>();
            shareLink.Should().BeAssignableTo<IEntity>();
        }

        [Fact]
        public void ShareLink_Should_Have_Valid_Id()
        {
            // Arrange & Act
            var shareLink = CreateTestShareLink();

            // Assert
            shareLink.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void ShareLink_Should_Support_Soft_Delete()
        {
            // Arrange
            var shareLink = CreateTestShareLink();

            // Act
            shareLink.SoftDelete();

            // Assert
            shareLink.IsDeleted.Should().BeTrue();
            shareLink.DeletionTime.Should().NotBeNull();
            shareLink.DeletionTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void ShareLink_Should_Track_Creation_Time()
        {
            // Arrange
            var beforeCreate = DateTime.Now;

            // Act
            var shareLink = CreateTestShareLink();
            var afterCreate = DateTime.Now;

            // Assert
            shareLink.CreationTime.Should().BeOnOrAfter(beforeCreate);
            shareLink.CreationTime.Should().BeOnOrBefore(afterCreate);
        }

        [Fact]
        public void ShareLink_Should_Track_Modification_Time()
        {
            // Arrange
            var shareLink = CreateTestShareLink();
            var originalModificationTime = shareLink.LastModificationTime;
            Thread.Sleep(10);

            // Act
            shareLink.NotifyModified();

            // Assert
            shareLink.LastModificationTime.Should().NotBe(originalModificationTime);
            shareLink.LastModificationTime.Should().NotBeNull();
        }

        [Fact]
        public void ShareLink_Should_Support_Domain_Events()
        {
            // Arrange
            var shareLink = CreateTestShareLink();
            var testEvent = new TestDomainEvent();

            // Act
            shareLink.AddNotification(testEvent);
            var events = shareLink.GetNotifications();

            // Assert
            events.Should().Contain(testEvent);
            events.Should().HaveCount(1);
        }

        [Fact]
        public void ShareLink_Should_Clear_Domain_Events()
        {
            // Arrange
            var shareLink = CreateTestShareLink();
            shareLink.AddNotification(new TestDomainEvent());
            shareLink.AddNotification(new TestDomainEvent());

            // Act
            shareLink.ClearNotifications();
            var events = shareLink.GetNotifications();

            // Assert
            events.Should().BeEmpty();
        }

        [Fact]
        public void ShareLink_Should_Be_Record_Type()
        {
            // Arrange
            var shareLink1 = CreateTestShareLink();
            var shareLink2 = CreateTestShareLink();

            // Assert
            // Record types should have value-based equality for their properties
            shareLink1.Should().NotBeSameAs(shareLink2);
            shareLink1.Id.Should().NotBe(shareLink2.Id); // Different instances have different IDs
        }

        [Fact]
        public void ShareLink_Should_Not_Add_Duplicate_Events()
        {
            // Arrange
            var shareLink = CreateTestShareLink();
            var testEvent = new TestDomainEvent();

            // Act
            shareLink.AddNotification(testEvent);
            shareLink.AddNotification(testEvent); // Try to add the same event again

            // Assert
            var events = shareLink.GetNotifications();
            events.Should().HaveCount(1);
        }

        private ShareLink CreateTestShareLink()
        {
            return ShareLink.Create(
                fileItemId: Guid.NewGuid(),
                creatorId: Guid.NewGuid(),
                title: "Test Share");
        }

        // Helper test event
        private class TestDomainEvent : MediatR.INotification
        {
        }
    }
}
