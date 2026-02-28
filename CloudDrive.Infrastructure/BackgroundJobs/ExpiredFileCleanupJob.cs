using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CloudDrive.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// 过期文件永久清理任务 — 清除回收站中超过保留期的软删除文件
    /// 默认保留 30 天
    /// </summary>
    [DisallowConcurrentExecution]
    public class ExpiredFileCleanupJob : IJob
    {
        /// <summary>
        /// 回收站文件保留天数
        /// </summary>
        private const int RetentionDays = 30;

        private readonly IFileRepository _fileRepository;
        private readonly IStorageProvider _storageProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExpiredFileCleanupJob> _logger;

        public ExpiredFileCleanupJob(
            IFileRepository fileRepository,
            IStorageProvider storageProvider,
            IUnitOfWork unitOfWork,
            ILogger<ExpiredFileCleanupJob> logger)
        {
            _fileRepository = fileRepository;
            _storageProvider = storageProvider;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行过期文件永久清理任务（保留期：{Days} 天）", RetentionDays);

            try
            {
                var cutoff = DateTime.Now.AddDays(-RetentionDays);
                var expiredFiles = await _fileRepository.GetExpiredDeletedFilesAsync(cutoff, 200);

                if (expiredFiles.Count == 0)
                {
                    _logger.LogInformation("没有需要永久清理的过期文件");
                    return;
                }

                var deletedCount = 0;
                var freedBytes = 0L;

                foreach (var file in expiredFiles)
                {
                    try
                    {
                        // 删除物理存储
                        if (!file.IsFolder)
                        {
                            await _storageProvider.DeleteAsync(file.StoragePath);
                            freedBytes += file.Size.bytesize;
                        }

                        // 永久删除数据库记录
                        await _fileRepository.DeleteAsync(file);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "清理文件 {FileId}（{FileName}）失败，跳过", file.Id, file.Name);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "过期文件清理完成，永久删除 {Count} 个文件，释放存储 {Bytes} 字节",
                    deletedCount, freedBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "过期文件清理任务执行失败");
                throw;
            }
        }
    }
}
