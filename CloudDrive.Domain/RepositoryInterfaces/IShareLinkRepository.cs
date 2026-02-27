using CloudDrive.Domain.Entities;

namespace CloudDrive.Domain.RepositoryInterfaces
{
    /// <summary>
    /// 分享链接仓储接口
    /// </summary>
    public interface IShareLinkRepository
    {
        /// <summary>
        /// 根据ID获取分享链接
        /// </summary>
        Task<ShareLink?> GetByIdAsync(Guid id);

        /// <summary>
        /// 根据分享码获取分享链接
        /// </summary>
        Task<ShareLink?> GetByShareCodeAsync(string shareCode);

        /// <summary>
        /// 获取用户创建的所有分享链接
        /// </summary>
        Task<List<ShareLink>> GetByCreatorIdAsync(Guid creatorId);

        /// <summary>
        /// 获取文件关联的所有分享链接
        /// </summary>
        Task<List<ShareLink>> GetByFileItemIdAsync(Guid fileItemId);

        /// <summary>
        /// 添加分享链接
        /// </summary>
        Task AddAsync(ShareLink shareLink);

        /// <summary>
        /// 更新分享链接
        /// </summary>
        Task UpdateAsync(ShareLink shareLink);

        /// <summary>
        /// 删除分享链接
        /// </summary>
        Task DeleteAsync(ShareLink shareLink);
    }
}
