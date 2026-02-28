using CloudDrive.Domain.Entities;
using CloudDrive.Domain.RepositoryInterfaces;
using CloudDrive.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CloudDrive.Infrastructure.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly CloudDriveDbContext _dbContext;

        public FileRepository(CloudDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FileItem?> GetByIdAsync(Guid id)
        {
            return await _dbContext.FileItems.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FileItem?> GetByHashAsync(FileHash hash)
        {
            return await _dbContext.FileItems
                .FirstOrDefaultAsync(f => f.Hash.hash == hash.hash);
        }

        public async Task<FileItem?> GetByHashAndOwnerAsync(FileHash hash, Guid ownerId)
        {
            return await _dbContext.FileItems
                .FirstOrDefaultAsync(f => f.Hash.hash == hash.hash && f.OwnerId == ownerId);
        }

        public async Task<bool> ExistsByHashAsync(FileHash hash)
        {
            return await _dbContext.FileItems
                .AnyAsync(f => f.Hash.hash == hash.hash);
        }

        public async Task<List<FileItem>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _dbContext.FileItems
                .Where(f => f.OwnerId == ownerId)
                .OrderByDescending(f => f.IsFolder)
                .ThenBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<List<FileItem>> GetByParentFolderIdAsync(Guid ownerId, Guid? parentFolderId)
        {
            return await _dbContext.FileItems
                .Where(f => f.OwnerId == ownerId && f.ParentFolderId == parentFolderId)
                .OrderByDescending(f => f.IsFolder)
                .ThenBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<long> GetTotalSizeByOwnerAsync(Guid ownerId)
        {
            return await _dbContext.FileItems
                .Where(f => f.OwnerId == ownerId && !f.IsFolder)
                .SumAsync(f => f.Size.bytesize);
        }

        public async Task AddAsync(FileItem fileItem)
        {
            await _dbContext.FileItems.AddAsync(fileItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(FileItem fileItem)
        {
            _dbContext.FileItems.Update(fileItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(FileItem fileItem)
        {
            _dbContext.FileItems.Remove(fileItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(List<FileItem> Items, int TotalCount)> GetPagedAsync(
            Guid ownerId,
            Guid? parentFolderId,
            int pageIndex,
            int pageSize,
            string? sortBy = null,
            bool ascending = true)
        {
            var query = _dbContext.FileItems
                .Where(f => f.OwnerId == ownerId && f.ParentFolderId == parentFolderId);

            // 总记录数
            var totalCount = await query.CountAsync();

            // 排序：文件夹始终在前
            IOrderedQueryable<FileItem> orderedQuery = query.OrderByDescending(f => f.IsFolder);

            orderedQuery = sortBy?.ToLowerInvariant() switch
            {
                "name" => ascending
                    ? orderedQuery.ThenBy(f => f.Name)
                    : orderedQuery.ThenByDescending(f => f.Name),
                "size" => ascending
                    ? orderedQuery.ThenBy(f => f.Size.bytesize)
                    : orderedQuery.ThenByDescending(f => f.Size.bytesize),
                "creationtime" => ascending
                    ? orderedQuery.ThenBy(f => f.CreationTime)
                    : orderedQuery.ThenByDescending(f => f.CreationTime),
                "lastmodificationtime" => ascending
                    ? orderedQuery.ThenBy(f => f.LastModificationTime)
                    : orderedQuery.ThenByDescending(f => f.LastModificationTime),
                _ => orderedQuery.ThenByDescending(f => f.CreationTime)
            };

            var items = await orderedQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<FileItem> Items, int TotalCount)> SearchAsync(
            Guid ownerId,
            string keyword,
            int pageIndex,
            int pageSize)
        {
            var query = _dbContext.FileItems
                .Where(f => f.OwnerId == ownerId && f.Name.Contains(keyword));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(f => f.IsFolder)
                .ThenByDescending(f => f.CreationTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<FileItem>> GetByExtensionsAsync(Guid ownerId, string[] extensions)
        {
            return await _dbContext.FileItems
                .Where(f => f.OwnerId == ownerId && !f.IsFolder && extensions.Contains(f.Extension))
                .OrderByDescending(f => f.CreationTime)
                .ToListAsync();
        }

        public async Task<List<FileItem>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var idList = ids.ToList();
            return await _dbContext.FileItems
                .Where(f => idList.Contains(f.Id))
                .ToListAsync();
        }

        public async Task<(List<FileItem> Items, int TotalCount)> GetDeletedByOwnerAsync(
            Guid ownerId,
            int pageIndex,
            int pageSize)
        {
            var query = _dbContext.FileItems
                .IgnoreQueryFilters()
                .Where(f => f.OwnerId == ownerId && f.IsDeleted);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(f => f.DeletionTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<FileItem?> GetDeletedByIdAsync(Guid id)
        {
            return await _dbContext.FileItems
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(f => f.Id == id && f.IsDeleted);
        }

        public async Task UpdateRangeAsync(IEnumerable<FileItem> fileItems)
        {
            _dbContext.FileItems.UpdateRange(fileItems);
            await _dbContext.SaveChangesAsync();
        }
    }
}
