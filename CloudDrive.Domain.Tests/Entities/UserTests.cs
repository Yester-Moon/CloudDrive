using CloudDrive.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace CloudDrive.Domain.Tests.Entities
{
    /// <summary>
    /// User 实体单元测试
    /// </summary>
    public class UserTests
    {
        [Fact]
        public void User_Should_Inherit_From_IdentityUser()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Should().BeAssignableTo<IdentityUser<Guid>>();
        }

        [Fact]
        public void User_Should_Have_Guid_As_Id_Type()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Id.GetType().Should().Be(typeof(Guid));
            user.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void User_Should_Have_Identity_Properties()
        {
            // Arrange & Act
            var user = new User
            {
                UserName = "testuser",
                Email = "test@example.com"
            };

            // Assert
            user.UserName.Should().Be("testuser");
            user.Email.Should().Be("test@example.com");
        }

        [Fact]
        public void User_Should_Initialize_With_Default_Values()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Id.Should().NotBeEmpty();
            user.UserName.Should().BeNull();
            user.Email.Should().BeNull();
        }

        [Fact]
        public void User_Should_Support_Email_Confirmation()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                EmailConfirmed = true
            };

            // Assert
            user.EmailConfirmed.Should().BeTrue();
        }

        [Fact]
        public void User_Should_Support_Phone_Number()
        {
            // Arrange
            var user = new User
            {
                PhoneNumber = "+86 138 0000 0000",
                PhoneNumberConfirmed = true
            };

            // Assert
            user.PhoneNumber.Should().Be("+86 138 0000 0000");
            user.PhoneNumberConfirmed.Should().BeTrue();
        }

        [Fact]
        public void User_Should_Support_Two_Factor_Authentication()
        {
            // Arrange
            var user = new User
            {
                TwoFactorEnabled = true
            };

            // Assert
            user.TwoFactorEnabled.Should().BeTrue();
        }

        [Fact]
        public void User_Should_Support_Lockout()
        {
            // Arrange
            var lockoutEnd = DateTimeOffset.Now.AddHours(1);
            var user = new User
            {
                LockoutEnabled = true,
                LockoutEnd = lockoutEnd
            };

            // Assert
            user.LockoutEnabled.Should().BeTrue();
            user.LockoutEnd.Should().Be(lockoutEnd);
        }

        [Fact]
        public void User_Should_Track_Access_Failed_Count()
        {
            // Arrange
            var user = new User
            {
                AccessFailedCount = 3
            };

            // Assert
            user.AccessFailedCount.Should().Be(3);
        }

        [Fact]
        public void User_Should_Support_Security_Stamp()
        {
            // Arrange
            var securityStamp = Guid.NewGuid().ToString();
            var user = new User
            {
                SecurityStamp = securityStamp
            };

            // Assert
            user.SecurityStamp.Should().Be(securityStamp);
        }
    }
}
