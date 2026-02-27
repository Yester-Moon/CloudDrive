using CloudDrive.Domain.Entities;

namespace CloudDrive.Domain.RepositoryInterfaces
{
    /// <summary>
    /// 用户仓储接口
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        Task<User?> GetByUserNameAsync(string userName);

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// 更新用户
        /// </summary>
        Task UpdateAsync(User user);
    }
}
