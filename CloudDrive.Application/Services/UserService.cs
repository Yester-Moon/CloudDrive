using CloudDrive.Application.Dtos;
using CloudDrive.Application.Interfaces;
using CloudDrive.Common.Exceptions;
using CloudDrive.Common.JWT;
using CloudDrive.Domain.Entities;
using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CloudDrive.Application.Services
{
    /// <summary>
    /// 用户应用服务
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly JWTOptions _jwtOptions;
        private readonly QuotaService _quotaService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            UserManager<User> userManager,
            IUserRepository userRepository,
            ITokenService tokenService,
            JWTOptions jwtOptions,
            QuotaService quotaService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _jwtOptions = jwtOptions;
            _quotaService = quotaService;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<UserDto> RegisterAsync(string userName, string email, string password)
        {
            // 检查用户名是否已存在
            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser != null)
                throw new DuplicateException("用户名已存在");

            // 检查邮箱是否已存在
            var existingEmail = await _userManager.FindByEmailAsync(email);
            if (existingEmail != null)
                throw new DuplicateException("邮箱已被注册");

            // 创建用户
            var user = User.Create(userName, email);
            user.Activate();

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new BusinessException($"注册失败：{errors}");
            }

            return MapToDto(user);
        }

        /// <inheritdoc />
        public async Task<(UserDto User, string Token)> LoginAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName)
                ?? throw new InvalidCredentialsException();

            if (user.IsBanned)
                throw new UserBannedException($"账户已被封禁：{user.BanReason}");

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isValid)
                throw new InvalidCredentialsException();

            // 记录登录时间
            user.RecordLogin();
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // 生成Token
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!)
            };

            var token = _tokenService.GenerateToken(claims, _jwtOptions);

            return (MapToDto(user), token);
        }

        /// <inheritdoc />
        public async Task<UserDto?> GetUserInfoAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return MapToDto(user);
        }

        /// <inheritdoc />
        public async Task UpdateDisplayNameAsync(Guid userId, string displayName)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            user.UpdateDisplayName(displayName);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAvatarAsync(Guid userId, string avatarUrl)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            user.UpdateAvatar(avatarUrl);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<QuotaDto> GetQuotaAsync(Guid userId)
        {
            var info = await _quotaService.GetQuotaInfoAsync(userId);

            return new QuotaDto
            {
                TotalQuota = info.TotalQuota,
                UsedSpace = info.UsedSpace,
                RemainingSpace = info.RemainingSpace,
                UsagePercentage = info.UsagePercentage,
                FormattedTotalQuota = FormatBytes(info.TotalQuota),
                FormattedUsedSpace = FormatBytes(info.UsedSpace),
                FormattedRemainingSpace = FormatBytes(info.RemainingSpace),
                VipLevel = info.VipLevel,
                MaxSingleFileSize = info.MaxSingleFileSize
            };
        }

        /// <inheritdoc />
        public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new UserNotFoundException(userId);

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new BusinessException($"修改密码失败：{errors}");
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl,
                VipLevel = user.GetEffectiveVipLevel(),
                VipExpirationTime = user.VipExpirationTime,
                IsVipActive = user.IsVipActive(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Quota = new QuotaDto
                {
                    TotalQuota = user.TotalQuota,
                    UsedSpace = user.UsedSpace,
                    RemainingSpace = user.GetRemainingSpace(),
                    UsagePercentage = user.GetSpaceUsagePercentage(),
                    FormattedTotalQuota = FormatBytes(user.TotalQuota),
                    FormattedUsedSpace = FormatBytes(user.UsedSpace),
                    FormattedRemainingSpace = FormatBytes(user.GetRemainingSpace()),
                    VipLevel = user.GetEffectiveVipLevel(),
                    MaxSingleFileSize = QuotaService.GetMaxSingleFileSize(user.GetEffectiveVipLevel())
                }
            };
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = ["B", "KB", "MB", "GB", "TB"];
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
