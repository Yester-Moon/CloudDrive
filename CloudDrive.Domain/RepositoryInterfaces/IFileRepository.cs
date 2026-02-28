using CloudDrive.Domain.Entities;
using CloudDrive.Domain.ValueObjects;

namespace CloudDrive.Domain.RepositoryInterfaces
{
    /// <summary>
    /// 文件仓储接口
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// 根据ID获取文件
        /// </summary>
        Task<FileItem?> GetByIdAsync(Guid id);

        /// <summary>
        /// 根据哈希值查找文件（用于去重）
        /// </summary>
        Task<FileItem?> GetByHashAsync(FileHash hash);

        /// <summary>
        /// 根据哈希值查找指定用户的文件（用于同用户去重）
        /// </summary>
        Task<FileItem?> GetByHashAndOwnerAsync(FileHash hash, Guid ownerId);

        /// <summary>
        /// 判断是否存在相同哈希的文件
        /// </summary>
        Task<bool> ExistsByHashAsync(FileHash hash);

        /// <summary>
        /// 获取用户的所有文件
        /// </summary>
        Task<List<FileItem>> GetByOwnerIdAsync(Guid ownerId);

        /// <summary>
        /// 获取指定文件夹下的文件列表
        /// </summary>
        Task<List<FileItem>> GetByParentFolderIdAsync(Guid ownerId, Guid? parentFolderId);

        /// <summary>
        /// 计算用户已使用空间（字节）
        /// </summary>
        Task<long> GetTotalSizeByOwnerAsync(Guid ownerId);

        /// <summary>
        /// 添加文件
        /// </summary>
        Task AddAsync(FileItem fileItem);

        /// <summary>
        /// 更新文件
        /// </summary>
        Task UpdateAsync(FileItem fileItem);

        /// <summary>
        /// 删除文件
        /// </summary>
        Task DeleteAsync(FileItem fileItem);

        /// <summary>
        /// 分页查询用户文件
        /// </summary>
        /// <param name="ownerId">所有者ID</param>
        /// <param name="parentFolderId">父文件夹ID</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="ascending">是否升序</param>
        Task<(List<FileItem> Items, int TotalCount)> GetPagedAsync(
            Guid ownerId,
            Guid? parentFolderId,
            int pageIndex,
            int pageSize,
            string? sortBy = null,
            bool ascending = true);

        /// <summary>
        /// 搜索文件（按名称模糊匹配）
        /// </summary>
        Task<(List<FileItem> Items, int TotalCount)> SearchAsync(
            Guid ownerId,
            string keyword,
            int pageIndex,
            int pageSize);

        /// <summary>
        /// 按文件类型筛选
        /// </summary>
        Task<List<FileItem>> GetByExtensionsAsync(Guid ownerId, string[] extensions);

        /// <summary>
        /// 根据多个ID批量获取文件
        /// </summary>
        Task<List<FileItem>> GetByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// 获取用户回收站中的已删除文件（分页）
        /// </summary>
        Task<(List<FileItem> Items, int TotalCount)> GetDeletedByOwnerAsync(
            Guid ownerId,
            int pageIndex,
            int pageSize);

        /// <summary>
        /// 根据ID获取已删除的文件（忽略全局查询过滤器）
        /// </summary>
        Task<FileItem?> GetDeletedByIdAsync(Guid id);

        /// <summary>
        /// 批量更新文件
        /// </summary>
        Task UpdateRangeAsync(IEnumerable<FileItem> fileItems);
    }
}
