using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CloudDrive.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// 存储空间统计同步任务 — 重新计算每个用户的实际已用空间并修正漂移
    /// </summary>
    [DisallowConcurrentExecution]
    public class StorageStatsSyncJob : IJob
    {
        private readonly IFileRepository _fileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StorageStatsSyncJob> _logger;

        public StorageStatsSyncJob(
            IFileRepository fileRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<StorageStatsSyncJob> logger)
        {
            _fileRepository = fileRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行存储空间统计同步任务");

            try
            {
                // 按用户聚合实际文件总大小
                var actualStats = await _fileRepository.GetStorageStatsByOwnerAsync();

                var correctedCount = 0;

                foreach (var (ownerId, actualBytes) in actualStats)
                {
                    var user = await _userRepository.GetByIdAsync(ownerId);
                    if (user == null) continue;

                    if (user.UsedSpace != actualBytes)
                    {
                        _logger.LogWarning(
                            "用户 {UserId} 存储漂移：记录值 {Recorded} 字节 → 实际值 {Actual} 字节",
                            ownerId, user.UsedSpace, actualBytes);

                        user.SyncUsedSpace(actualBytes);
                        await _userRepository.UpdateAsync(user);
                        correctedCount++;
                    }
                }

                if (correctedCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                }

                _logger.LogInformation("存储空间统计同步完成，修正 {Count} 个用户", correctedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "存储空间统计同步任务执行失败");
                throw;
            }
        }
    }
}
