using CloudDrive.Domain.DomainEvents;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CloudDrive.Domain.Tests.DomainEvents
{
    /// <summary>
    /// QuotaChangedEvent 领域事件单元测试
    /// </summary>
    public class QuotaChangedEventTests
    {
        [Fact]
        public void QuotaChangedEvent_Should_Implement_INotification()
        {
            // Arrange & Act
            var quotaChangedEvent = new QuotaChangedEvent(Guid.NewGuid(), 1024, 10240);

            // Assert
            quotaChangedEvent.Should().BeAssignableTo<INotification>();
        }

        [Fact]
        public void QuotaChangedEvent_Should_Create_With_Valid_Properties()
        {
            // Arrange
            var userId = Guid.NewGuid();
            long currentUsedSpace = 5 * 1024 * 1024 * 1024L; // 5GB
            long totalQuota = 10 * 1024 * 1024 * 1024L; // 10GB
            var beforeCreate = DateTime.Now;

            // Act
            var quotaChangedEvent = new QuotaChangedEvent(userId, currentUsedSpace, totalQuota);
            var afterCreate = DateTime.Now;

            // Assert
            quotaChangedEvent.UserId.Should().Be(userId);
            quotaChangedEvent.CurrentUsedSpace.Should().Be(currentUsedSpace);
            quotaChangedEvent.TotalQuota.Should().Be(totalQuota);
            quotaChangedEvent.OccurredOn.Should().BeOnOrAfter(beforeCreate);
            quotaChangedEvent.OccurredOn.Should().BeOnOrBefore(afterCreate);
        }

        [Fact]
        public void GetRemainingSpace_Should_Calculate_Correctly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            long currentUsedSpace = 3 * 1024 * 1024 * 1024L; // 3GB
            long totalQuota = 10 * 1024 * 1024 * 1024L; // 10GB
            var quotaChangedEvent = new QuotaChangedEvent(userId, currentUsedSpace, totalQuota);

            // Act
            long remainingSpace = quotaChangedEvent.GetRemainingSpace();

            // Assert
            remainingSpace.Should().Be(7 * 1024 * 1024 * 1024L); // 7GB
        }

        [Fact]
        public void GetRemainingSpace_Should_Return_Zero_When_Quota_Exceeded()
        {
            // Arrange
            var userId = Guid.NewGuid();
            long currentUsedSpace = 12 * 1024 * 1024 * 1024L; // 12GB
            long totalQuota = 10 * 1024 * 1024 * 1024L; // 10GB
            var quotaChangedEvent = new QuotaChangedEvent(userId, currentUsedSpace, totalQuota);

            // Act
            long remainingSpace = quotaChangedEvent.GetRemainingSpace();

            // Assert
            remainingSpace.Should().Be(0);
        }

        [Fact]
        public void GetRemainingSpace_Should_Return_TotalQuota_When_No_Space_Used()
        {
            // Arrange
            var userId = Guid.NewGuid();
            long currentUsedSpace = 0;
            long totalQuota = 10 * 1024 * 1024 * 1024L; // 10GB
            var quotaChangedEvent = new QuotaChangedEvent(userId, currentUsedSpace, totalQuota);

            // Act
            long remainingSpace = quotaChangedEvent.GetRemainingSpace();

            // Assert
            remainingSpace.Should().Be(totalQuota);
        }

        [Theory]
        [InlineData(0, 10240, 0)]
        [InlineData(5120, 10240, 50)]
        [InlineData(10240, 10240, 100)]
        [InlineData(7680, 10240, 75)]
        [InlineData(2560, 10240, 25)]
        public void GetUsagePercentage_Should_Calculate_Correctly(long used, long total, double expectedPercentage)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var quotaChangedEvent = new QuotaChangedEvent(userId, used, total);

            // Act
            double usagePercentage = quotaChangedEvent.GetUsagePercentage();

            // Assert
            usagePercentage.Should().BeApproximately(expectedPercentage, 0.01);
        }

        [Fact]
        public void GetUsagePercentage_Should_Return_Zero_When_TotalQuota_Is_Zero()
        {
            // Arrange
            var userId = Guid.NewGuid();
            long currentUsedSpace = 1024;
            long totalQuota = 0;
            var quotaChangedEvent = new QuotaChangedEvent(userId, currentUsedSpace, totalQuota);

            // Act
            double usagePercentage = quotaChangedEvent.GetUsagePercentage();

            // Assert
            usagePercentage.Should().Be(0);
        }

        [Fact]
        public void GetUsagePercentage_Should_Handle_Over_100_Percent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            long currentUsedSpace = 15 * 1024 * 1024 * 1024L; // 15GB
            long totalQuota = 10 * 1024 * 1024 * 1024L; // 10GB
            var quotaChangedEvent = new QuotaChangedEvent(userId, currentUsedSpace, totalQuota);

            // Act
            double usagePercentage = quotaChangedEvent.GetUsagePercentage();

            // Assert
            usagePercentage.Should().BeGreaterThan(100);
            usagePercentage.Should().BeApproximately(150, 0.01);
        }
    }
}
