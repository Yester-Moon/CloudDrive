using CloudDrive.Domain.Entities;
using CloudDrive.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudDrive.Infrastructure.Repositories
{
    public class ShareLinkRepository : IShareLinkRepository
    {
        private readonly CloudDriveDbContext _dbContext;

        public ShareLinkRepository(CloudDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShareLink?> GetByIdAsync(Guid id)
        {
            return await _dbContext.ShareLinks.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<ShareLink?> GetByShareCodeAsync(string shareCode)
        {
            return await _dbContext.ShareLinks
                .FirstOrDefaultAsync(s => s.ShareCode == shareCode);
        }

        public async Task<List<ShareLink>> GetByCreatorIdAsync(Guid creatorId)
        {
            return await _dbContext.ShareLinks
                .Where(s => s.CreatorId == creatorId)
                .OrderByDescending(s => s.CreationTime)
                .ToListAsync();
        }

        public async Task<List<ShareLink>> GetByFileItemIdAsync(Guid fileItemId)
        {
            return await _dbContext.ShareLinks
                .Where(s => s.FileItemId == fileItemId)
                .OrderByDescending(s => s.CreationTime)
                .ToListAsync();
        }

        public async Task AddAsync(ShareLink shareLink)
        {
            await _dbContext.ShareLinks.AddAsync(shareLink);
        }

        public Task UpdateAsync(ShareLink shareLink)
        {
            _dbContext.ShareLinks.Update(shareLink);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ShareLink shareLink)
        {
            _dbContext.ShareLinks.Remove(shareLink);
            return Task.CompletedTask;
        }

        public async Task<List<ShareLink>> GetExpiredActiveLinksAsync(int batchSize = 100)
        {
            return await _dbContext.ShareLinks
                .Where(s => !s.IsCancelled
                    && s.ExpirationTime.HasValue
                    && s.ExpirationTime.Value < DateTime.Now)
                .Take(batchSize)
                .ToListAsync();
        }

        public Task UpdateRangeAsync(IEnumerable<ShareLink> shareLinks)
        {
            _dbContext.ShareLinks.UpdateRange(shareLinks);
            return Task.CompletedTask;
        }
    }
}
