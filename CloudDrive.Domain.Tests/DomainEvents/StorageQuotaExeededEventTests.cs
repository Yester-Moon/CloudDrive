using CloudDrive.Domain.DomainEvents;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CloudDrive.Domain.Tests.DomainEvents
{
    /// <summary>
    /// StorageQuotaExeededEvent 领域事件单元测试
    /// </summary>
    public class StorageQuotaExeededEventTests
    {
        [Fact]
        public void StorageQuotaExeededEvent_Should_Implement_INotification()
        {
            // Arrange & Act
            var quotaExeededEvent = new StorageQuotaExeededEvent(
                Guid.NewGuid(), 
                9 * 1024 * 1024 * 1024L, 
                10 * 1024 * 1024 * 1024L, 
                2 * 1024 * 1024 * 1024L);

            // Assert
            quotaExeededEvent.Should().BeAssignableTo<INotification>();
        }

        [Fact]
        public void StorageQuotaExeededEvent_Should_Create_With_Valid_Properties()
        {
            // Arrange
            var userId = Guid.NewGuid();
            long currentUsedSpace = 9 * 1024 * 1024 * 1024L; // 9GB
            long totalQuota = 10 * 1024 * 1024 * 1024L; // 10GB
            long attemptedSize = 2 * 1024 * 1024 * 1024L; // 2GB
            var beforeCreate = DateTime.Now;

            // Act
            var quotaExeededEvent = new StorageQuotaExeededEvent(
                userId, currentUsedSpace, totalQuota, attemptedSize);
            var afterCreate = DateTime.Now;

            // Assert
            quotaExeededEvent.UserId.Should().Be(userId);
            quotaExeededEvent.CurrentUsedSpace.Should().Be(currentUsedSpace);
            quotaExeededEvent.TotalQuota.Should().Be(totalQuota);
            quotaExeededEvent.AttemptedSize.Should().Be(attemptedSize);
            quotaExeededEvent.OccurredOn.Should().BeOnOrAfter(beforeCreate);
            quotaExeededEvent.OccurredOn.Should().BeOnOrBefore(afterCreate);
        }

        [Fact]
        public void StorageQuotaExeededEvent_Should_Track_Attempted_Upload_Size()
        {
            // Arrange
            var userId = Guid.NewGuid();
            long currentUsedSpace = (long)(9.5 * 1024 * 1024 * 1024); // 9.5GB
            long totalQuota = 10 * 1024 * 1024 * 1024L; // 10GB
            long attemptedSize = 1 * 1024 * 1024 * 1024L; // 1GB

            // Act
            var quotaExeededEvent = new StorageQuotaExeededEvent(
                userId, currentUsedSpace, totalQuota, attemptedSize);

            // Assert
            quotaExeededEvent.AttemptedSize.Should().Be(attemptedSize);
            (quotaExeededEvent.CurrentUsedSpace + quotaExeededEvent.AttemptedSize)
                .Should().BeGreaterThan(quotaExeededEvent.TotalQuota);
        }

        [Theory]
        [InlineData(9 * 1024 * 1024 * 1024L, 10 * 1024 * 1024 * 1024L, 2 * 1024 * 1024 * 1024L)]
        [InlineData(10 * 1024 * 1024 * 1024L, 10 * 1024 * 1024 * 1024L, 1)]
        [InlineData(5 * 1024 * 1024 * 1024L, 10 * 1024 * 1024 * 1024L, 6 * 1024 * 1024 * 1024L)]
        public void StorageQuotaExeededEvent_Should_Handle_Various_Quota_Scenarios(
            long currentUsed, long total, long attempted)
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var quotaExeededEvent = new StorageQuotaExeededEvent(
                userId, currentUsed, total, attempted);

            // Assert
            quotaExeededEvent.CurrentUsedSpace.Should().Be(currentUsed);
            quotaExeededEvent.TotalQuota.Should().Be(total);
            quotaExeededEvent.AttemptedSize.Should().Be(attempted);
        }
    }
}
