using CloudDrive.Common.Models;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CloudDrive.Domain.Tests.Common
{
    /// <summary>
    /// BaseEntity 基础实体单元测试
    /// </summary>
    public class BaseEntityTests
    {
        [Fact]
        public void BaseEntity_Should_Generate_Unique_Id()
        {
            // Arrange & Act
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();

            // Assert
            entity1.Id.Should().NotBe(Guid.Empty);
            entity2.Id.Should().NotBe(Guid.Empty);
            entity1.Id.Should().NotBe(entity2.Id);
        }

        [Fact]
        public void BaseEntity_Should_Implement_IEntity()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            entity.Should().BeAssignableTo<IEntity>();
        }

        [Fact]
        public void BaseEntity_Should_Implement_IDomainEvents()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            entity.Should().BeAssignableTo<IDomainEvents>();
        }

        [Fact]
        public void AddNotification_Should_Add_Event()
        {
            // Arrange
            var entity = new TestEntity();
            var notification = new TestNotification();

            // Act
            entity.AddNotification(notification);

            // Assert
            var events = entity.GetNotifications();
            events.Should().Contain(notification);
            events.Should().HaveCount(1);
        }

        [Fact]
        public void AddNotification_Should_Not_Add_Duplicate_Events()
        {
            // Arrange
            var entity = new TestEntity();
            var notification = new TestNotification();

            // Act
            entity.AddNotification(notification);
            entity.AddNotification(notification); // Try to add same event

            // Assert
            var events = entity.GetNotifications();
            events.Should().HaveCount(1);
        }

        [Fact]
        public void ClearNotifications_Should_Remove_All_Events()
        {
            // Arrange
            var entity = new TestEntity();
            entity.AddNotification(new TestNotification());
            entity.AddNotification(new TestNotification());

            // Act
            entity.ClearNotifications();

            // Assert
            entity.GetNotifications().Should().BeEmpty();
        }

        [Fact]
        public void GetNotifications_Should_Return_All_Events()
        {
            // Arrange
            var entity = new TestEntity();
            var notification1 = new TestNotification();
            var notification2 = new TestNotification();

            // Act
            entity.AddNotification(notification1);
            entity.AddNotification(notification2);

            // Assert
            var events = entity.GetNotifications();
            events.Should().HaveCount(2);
            events.Should().Contain(notification1);
            events.Should().Contain(notification2);
        }

        // Test entity class
        private record TestEntity : BaseEntity
        {
        }

        // Test notification class
        private class TestNotification : INotification
        {
        }
    }
}
