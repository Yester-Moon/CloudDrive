using CloudDrive.Common.Models;
using CloudDrive.Domain.Entities;
using CloudDrive.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CloudDrive.Domain.Tests.Entities
{
    /// <summary>
    /// FileItem 实体单元测试
    /// </summary>
    public class FileItemTests
    {
        [Fact]
        public void FileItem_Should_Inherit_From_AggregateRootEntity()
        {
            // Arrange
            var fileItem = CreateTestFileItem();

            // Assert
            fileItem.Should().BeAssignableTo<AggregateRootEntity>();
            fileItem.Should().BeAssignableTo<IAggregateRoot>();
            fileItem.Should().BeAssignableTo<IEntity>();
        }

        [Fact]
        public void FileItem_Should_Have_Valid_Id()
        {
            // Arrange & Act
            var fileItem = CreateTestFileItem();

            // Assert
            fileItem.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void FileItem_Should_Support_Soft_Delete()
        {
            // Arrange
            var fileItem = CreateTestFileItem();

            // Act
            fileItem.SoftDelete();

            // Assert
            fileItem.IsDeleted.Should().BeTrue();
            fileItem.DeletionTime.Should().NotBeNull();
            fileItem.DeletionTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void FileItem_Should_Track_Creation_Time()
        {
            // Arrange
            var beforeCreate = DateTime.Now;

            // Act
            var fileItem = CreateTestFileItem();
            var afterCreate = DateTime.Now;

            // Assert
            fileItem.CreationTime.Should().BeOnOrAfter(beforeCreate);
            fileItem.CreationTime.Should().BeOnOrBefore(afterCreate);
        }

        [Fact]
        public void FileItem_Should_Track_Modification_Time()
        {
            // Arrange
            var fileItem = CreateTestFileItem();
            var originalModificationTime = fileItem.LastModificationTime;
            Thread.Sleep(10);

            // Act
            fileItem.NotifyModified();

            // Assert
            fileItem.LastModificationTime.Should().NotBe(originalModificationTime);
            fileItem.LastModificationTime.Should().NotBeNull();
        }

        [Fact]
        public void FileItem_Should_Support_Domain_Events()
        {
            // Arrange
            var fileItem = CreateTestFileItem();
            var testEvent = new TestDomainEvent();

            // Act
            fileItem.AddNotification(testEvent);
            var events = fileItem.GetNotifications();

            // Assert
            events.Should().Contain(testEvent);
            events.Should().HaveCount(1);
        }

        [Fact]
        public void FileItem_Should_Clear_Domain_Events()
        {
            // Arrange
            var fileItem = CreateTestFileItem();
            fileItem.AddNotification(new TestDomainEvent());
            fileItem.AddNotification(new TestDomainEvent());

            // Act
            fileItem.ClearNotifications();
            var events = fileItem.GetNotifications();

            // Assert
            events.Should().BeEmpty();
        }

        [Fact]
        public void FileItem_Should_Be_Record_Type()
        {
            // Arrange
            var fileItem1 = CreateTestFileItem();
            var fileItem2 = CreateTestFileItem();

            // Assert
            // Record types should have value-based equality for their properties
            fileItem1.Should().NotBeSameAs(fileItem2);
            fileItem1.Id.Should().NotBe(fileItem2.Id); // Different instances have different IDs
        }

        private FileItem CreateTestFileItem()
        {
            return FileItem.CreateFile(
                name: "test.pdf",
                size: new FileSize(1024),
                storagePath: new FilePath("/uploads/test.pdf"),
                hash: new FileHash("abc123"),
                mimeType: "application/pdf",
                ownerId: Guid.NewGuid());
        }

        // Helper test event
        private class TestDomainEvent : MediatR.INotification
        {
        }
    }
}
