using CloudDrive.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CloudDrive.Domain.Tests.ValueObjects
{
    /// <summary>
    /// FileSize 值对象单元测试
    /// </summary>
    public class FileSizeTests
    {
        [Fact]
        public void FileSize_Should_Create_With_Valid_ByteSize()
        {
            // Arrange
            long byteSize = 1024 * 1024; // 1MB

            // Act
            var fileSize = new FileSize(byteSize);

            // Assert
            fileSize.Should().NotBeNull();
            fileSize.bytesize.Should().Be(byteSize);
        }

        [Fact]
        public void FileSize_Should_Be_Equal_When_ByteSize_Is_Same()
        {
            // Arrange
            var fileSize1 = new FileSize(1024);
            var fileSize2 = new FileSize(1024);

            // Act & Assert
            fileSize1.Should().Be(fileSize2);
            (fileSize1 == fileSize2).Should().BeTrue();
        }

        [Fact]
        public void FileSize_Should_Not_Be_Equal_When_ByteSize_Is_Different()
        {
            // Arrange
            var fileSize1 = new FileSize(1024);
            var fileSize2 = new FileSize(2048);

            // Act & Assert
            fileSize1.Should().NotBe(fileSize2);
            (fileSize1 != fileSize2).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1024)]
        [InlineData(1024 * 1024)]
        [InlineData(1024L * 1024 * 1024)]
        public void FileSize_Should_Handle_Various_Sizes(long byteSize)
        {
            // Act
            var fileSize = new FileSize(byteSize);

            // Assert
            fileSize.bytesize.Should().Be(byteSize);
        }
    }
}
