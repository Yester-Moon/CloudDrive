using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CloudDrive.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// 过期分享清理任务 — 自动取消已过期的分享链接
    /// </summary>
    [DisallowConcurrentExecution]
    public class ExpiredShareCleanupJob : IJob
    {
        private readonly IShareLinkRepository _shareLinkRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExpiredShareCleanupJob> _logger;

        public ExpiredShareCleanupJob(
            IShareLinkRepository shareLinkRepository,
            IUnitOfWork unitOfWork,
            ILogger<ExpiredShareCleanupJob> logger)
        {
            _shareLinkRepository = shareLinkRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始执行过期分享清理任务");

            try
            {
                var expiredLinks = await _shareLinkRepository.GetExpiredActiveLinksAsync(200);

                if (expiredLinks.Count == 0)
                {
                    _logger.LogInformation("没有需要清理的过期分享链接");
                    return;
                }

                foreach (var link in expiredLinks)
                {
                    link.Cancel();
                }

                await _shareLinkRepository.UpdateRangeAsync(expiredLinks);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("过期分享清理完成，共取消 {Count} 个过期链接", expiredLinks.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "过期分享清理任务执行失败");
                throw;
            }
        }
    }
}
