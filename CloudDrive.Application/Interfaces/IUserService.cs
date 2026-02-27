using CloudDrive.Application.Dtos;

namespace CloudDrive.Application.Interfaces
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 用户注册
        /// </summary>
        Task<UserDto> RegisterAsync(string userName, string email, string password);

        /// <summary>
        /// 用户登录
        /// </summary>
        Task<(UserDto User, string Token)> LoginAsync(string userName, string password);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        Task<UserDto?> GetUserInfoAsync(Guid userId);

        /// <summary>
        /// 更新用户显示名称
        /// </summary>
        Task UpdateDisplayNameAsync(Guid userId, string displayName);

        /// <summary>
        /// 更新用户头像
        /// </summary>
        Task UpdateAvatarAsync(Guid userId, string avatarUrl);

        /// <summary>
        /// 获取配额信息
        /// </summary>
        Task<QuotaDto> GetQuotaAsync(Guid userId);

        /// <summary>
        /// 修改密码
        /// </summary>
        Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    }
}
