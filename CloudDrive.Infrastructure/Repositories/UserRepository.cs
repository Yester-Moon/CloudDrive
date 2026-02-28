using CloudDrive.Domain.Entities;
using CloudDrive.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudDrive.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CloudDriveDbContext _dbContext;

        public UserRepository(CloudDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            var normalized = userName.ToUpperInvariant();
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.NormalizedUserName == normalized);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == id);
        }

        public Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}
