using CloudDrive.Domain.Entities;
using CloudDrive.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudDrive.Infrastructure.Repositories
{
    public class ChunkUploadRepository : IChunkUploadRepository
    {
        private readonly CloudDriveDbContext _dbContext;

        public ChunkUploadRepository(CloudDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChunkUploadSession?> GetByIdAsync(Guid id)
        {
            return await _dbContext.ChunkUploadSessions.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(ChunkUploadSession session)
        {
            await _dbContext.ChunkUploadSessions.AddAsync(session);
        }

        public Task UpdateAsync(ChunkUploadSession session)
        {
            _dbContext.ChunkUploadSessions.Update(session);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ChunkUploadSession session)
        {
            _dbContext.ChunkUploadSessions.Remove(session);
            return Task.CompletedTask;
        }

        public async Task<List<ChunkUploadSession>> GetActiveByOwnerAsync(Guid ownerId)
        {
            return await _dbContext.ChunkUploadSessions
                .Where(s => s.OwnerId == ownerId && s.Status == ChunkUploadSession.StatusUploading)
                .OrderByDescending(s => s.CreationTime)
                .ToListAsync();
        }

        public async Task<List<ChunkUploadSession>> GetExpiredSessionsAsync()
        {
            return await _dbContext.ChunkUploadSessions
                .Where(s => s.Status == ChunkUploadSession.StatusUploading && s.ExpiresAt < DateTime.Now)
                .ToListAsync();
        }
    }
}
