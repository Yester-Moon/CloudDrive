using CloudDrive.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CloudDrive.Domain.Tests.ValueObjects
{
    /// <summary>
    /// FilePath 值对象单元测试
    /// </summary>
    public class FilePathTests
    {
        [Fact]
        public void FilePath_Should_Create_With_Valid_Path()
        {
            // Arrange
            string path = "/uploads/2024/01/test.pdf";

            // Act
            var filePath = new FilePath(path);

            // Assert
            filePath.Should().NotBeNull();
            filePath.path.Should().Be(path);
        }

        [Fact]
        public void FilePath_Should_Be_Equal_When_Path_Is_Same()
        {
            // Arrange
            var filePath1 = new FilePath("/uploads/test.pdf");
            var filePath2 = new FilePath("/uploads/test.pdf");

            // Act & Assert
            filePath1.Should().Be(filePath2);
            (filePath1 == filePath2).Should().BeTrue();
        }

        [Fact]
        public void FilePath_Should_Not_Be_Equal_When_Path_Is_Different()
        {
            // Arrange
            var filePath1 = new FilePath("/uploads/test1.pdf");
            var filePath2 = new FilePath("/uploads/test2.pdf");

            // Act & Assert
            filePath1.Should().NotBe(filePath2);
            (filePath1 != filePath2).Should().BeTrue();
        }

        [Theory]
        [InlineData("/uploads/test.pdf")]
        [InlineData("C:\\Users\\test\\file.txt")]
        [InlineData("uploads/2024/01/15/abc123.jpg")]
        [InlineData("/")]
        public void FilePath_Should_Handle_Various_Path_Formats(string path)
        {
            // Act
            var filePath = new FilePath(path);

            // Assert
            filePath.path.Should().Be(path);
        }
    }
}
