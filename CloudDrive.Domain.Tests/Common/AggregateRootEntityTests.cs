using CloudDrive.Common.Models;
using FluentAssertions;
using Xunit;

namespace CloudDrive.Domain.Tests.Common
{
    /// <summary>
    /// AggregateRootEntity 聚合根单元测试
    /// </summary>
    public class AggregateRootEntityTests
    {
        [Fact]
        public void AggregateRootEntity_Should_Inherit_From_BaseEntity()
        {
            // Arrange & Act
            var entity = new TestAggregateRoot();

            // Assert
            entity.Should().BeAssignableTo<BaseEntity>();
            entity.Should().BeAssignableTo<IAggregateRoot>();
        }

        [Fact]
        public void AggregateRootEntity_Should_Implement_ISoftDelete()
        {
            // Arrange & Act
            var entity = new TestAggregateRoot();

            // Assert
            entity.Should().BeAssignableTo<ISoftDelete>();
        }

        [Fact]
        public void AggregateRootEntity_Should_Implement_Time_Tracking_Interfaces()
        {
            // Arrange & Act
            var entity = new TestAggregateRoot();

            // Assert
            entity.Should().BeAssignableTo<IHasCreationTime>();
            entity.Should().BeAssignableTo<IHasDeletionTime>();
            entity.Should().BeAssignableTo<IHasModificationTime>();
        }

        [Fact]
        public void SoftDelete_Should_Mark_Entity_As_Deleted()
        {
            // Arrange
            var entity = new TestAggregateRoot();

            // Act
            entity.SoftDelete();

            // Assert
            entity.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void SoftDelete_Should_Set_DeletionTime()
        {
            // Arrange
            var entity = new TestAggregateRoot();
            var beforeDelete = DateTime.Now;

            // Act
            entity.SoftDelete();
            var afterDelete = DateTime.Now;

            // Assert
            entity.DeletionTime.Should().NotBeNull();
            entity.DeletionTime.Should().BeOnOrAfter(beforeDelete);
            entity.DeletionTime.Should().BeOnOrBefore(afterDelete);
        }

        [Fact]
        public void CreationTime_Should_Be_Set_On_Creation()
        {
            // Arrange
            var beforeCreate = DateTime.Now;

            // Act
            var entity = new TestAggregateRoot();
            var afterCreate = DateTime.Now;

            // Assert
            entity.CreationTime.Should().BeOnOrAfter(beforeCreate);
            entity.CreationTime.Should().BeOnOrBefore(afterCreate);
        }

        [Fact]
        public void NotifyModified_Should_Set_LastModificationTime()
        {
            // Arrange
            var entity = new TestAggregateRoot();
            Thread.Sleep(10);
            var beforeModify = DateTime.Now;

            // Act
            entity.NotifyModified();
            var afterModify = DateTime.Now;

            // Assert
            entity.LastModificationTime.Should().NotBeNull();
            entity.LastModificationTime.Should().BeOnOrAfter(beforeModify);
            entity.LastModificationTime.Should().BeOnOrBefore(afterModify);
        }

        [Fact]
        public void NotifyModified_Should_Update_LastModificationTime_On_Multiple_Calls()
        {
            // Arrange
            var entity = new TestAggregateRoot();
            entity.NotifyModified();
            var firstModificationTime = entity.LastModificationTime!.Value;
            Thread.Sleep(10);

            // Act
            entity.NotifyModified();

            // Assert
            entity.LastModificationTime.Should().NotBe(firstModificationTime);
            entity.LastModificationTime.Should().BeAfter(firstModificationTime);
        }

        [Fact]
        public void IsDeleted_Should_Be_False_By_Default()
        {
            // Arrange & Act
            var entity = new TestAggregateRoot();

            // Assert
            entity.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void DeletionTime_Should_Be_Null_By_Default()
        {
            // Arrange & Act
            var entity = new TestAggregateRoot();

            // Assert
            entity.DeletionTime.Should().BeNull();
        }

        [Fact]
        public void LastModificationTime_Should_Be_Null_By_Default()
        {
            // Arrange & Act
            var entity = new TestAggregateRoot();

            // Assert
            entity.LastModificationTime.Should().BeNull();
        }

        // Test aggregate root class
        private record TestAggregateRoot : AggregateRootEntity
        {
        }
    }
}
