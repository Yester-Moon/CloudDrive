using CloudDrive.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CloudDrive.Domain.Tests.ValueObjects
{
    /// <summary>
    /// FileHash 值对象单元测试
    /// </summary>
    public class FileHashTests
    {
        [Fact]
        public void FileHash_Should_Create_With_Valid_Hash()
        {
            // Arrange
            string hash = "5d41402abc4b2a76b9719d911017c592"; // MD5 hash example

            // Act
            var fileHash = new FileHash(hash);

            // Assert
            fileHash.Should().NotBeNull();
            fileHash.hash.Should().Be(hash);
        }

        [Fact]
        public void FileHash_Should_Be_Equal_When_Hash_Is_Same()
        {
            // Arrange
            var hash = "5d41402abc4b2a76b9719d911017c592";
            var fileHash1 = new FileHash(hash);
            var fileHash2 = new FileHash(hash);

            // Act & Assert
            fileHash1.Should().Be(fileHash2);
            (fileHash1 == fileHash2).Should().BeTrue();
        }

        [Fact]
        public void FileHash_Should_Not_Be_Equal_When_Hash_Is_Different()
        {
            // Arrange
            var fileHash1 = new FileHash("5d41402abc4b2a76b9719d911017c592");
            var fileHash2 = new FileHash("098f6bcd4621d373cade4e832627b4f6");

            // Act & Assert
            fileHash1.Should().NotBe(fileHash2);
            (fileHash1 != fileHash2).Should().BeTrue();
        }

        [Theory]
        [InlineData("5d41402abc4b2a76b9719d911017c592")] // MD5
        [InlineData("2fd4e1c67a2d28fced849ee1bb76e7391b93eb12")] // SHA-1
        [InlineData("d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592")] // SHA-256
        public void FileHash_Should_Handle_Various_Hash_Algorithms(string hash)
        {
            // Act
            var fileHash = new FileHash(hash);

            // Assert
            fileHash.hash.Should().Be(hash);
        }

        [Fact]
        public void FileHash_Should_Be_Case_Sensitive()
        {
            // Arrange
            var fileHash1 = new FileHash("5D41402ABC4B2A76B9719D911017C592");
            var fileHash2 = new FileHash("5d41402abc4b2a76b9719d911017c592");

            // Act & Assert
            fileHash1.Should().NotBe(fileHash2);
        }
    }
}
