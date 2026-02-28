using CloudDrive.Domain.Entities;

namespace CloudDrive.Domain.RepositoryInterfaces
{
    /// <summary>
    /// 分片上传会话仓储接口
    /// </summary>
    public interface IChunkUploadRepository
    {
        /// <summary>
        /// 根据ID获取会话
        /// </summary>
        Task<ChunkUploadSession?> GetByIdAsync(Guid id);

        /// <summary>
        /// 添加会话
        /// </summary>
        Task AddAsync(ChunkUploadSession session);

        /// <summary>
        /// 更新会话
        /// </summary>
        Task UpdateAsync(ChunkUploadSession session);

        /// <summary>
        /// 删除会话
        /// </summary>
        Task DeleteAsync(ChunkUploadSession session);

        /// <summary>
        /// 获取用户的活跃上传会话列表
        /// </summary>
        Task<List<ChunkUploadSession>> GetActiveByOwnerAsync(Guid ownerId);

        /// <summary>
        /// 获取所有已过期的会话
        /// </summary>
        Task<List<ChunkUploadSession>> GetExpiredSessionsAsync();
    }
}
