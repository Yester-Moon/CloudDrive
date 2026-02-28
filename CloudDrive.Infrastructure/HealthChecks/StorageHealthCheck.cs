using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.ValueObjects;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CloudDrive.Infrastructure.HealthChecks
{
    /// <summary>
    /// 存储提供者健康检查 — 验证文件存储读写是否正常
    /// </summary>
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IStorageProvider _storageProvider;
        private readonly ILogger<StorageHealthCheck> _logger;

        public StorageHealthCheck(IStorageProvider storageProvider, ILogger<StorageHealthCheck> logger)
        {
            _storageProvider = storageProvider;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // 写入一个临时探针文件
                var probeContent = $"health-check-{DateTime.UtcNow:O}";
                using var writeStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(probeContent));

                var probePath = await _storageProvider.UploadAsync(writeStream, ".health-probe.txt", "text/plain");

                // 验证文件存在
                var exists = await _storageProvider.ExistsAsync(probePath);
                if (!exists)
                {
                    return HealthCheckResult.Degraded("存储探针文件上传后未找到");
                }

                // 清理探针文件
                await _storageProvider.DeleteAsync(probePath);

                return HealthCheckResult.Healthy("存储读写正常");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "存储健康检查失败");
                return HealthCheckResult.Unhealthy("存储不可用", ex);
            }
        }
    }
}
