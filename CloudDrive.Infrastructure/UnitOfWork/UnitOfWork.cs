using Microsoft.EntityFrameworkCore.Storage;

namespace CloudDrive.Infrastructure.UnitOfWork
{
    /// <summary>
    /// 工作单元实现
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CloudDriveDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(CloudDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("没有活动的事务");

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null) return;

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
