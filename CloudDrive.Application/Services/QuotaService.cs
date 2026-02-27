using CloudDrive.Domain.Entities;
using CloudDrive.Domain.RepositoryInterfaces;

namespace CloudDrive.Application.Services
{
    /// <summary>
    /// 配额计算领域服务
    /// </summary>
    public class QuotaService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileRepository _fileRepository;

        public QuotaService(IUserRepository userRepository, IFileRepository fileRepository)
        {
            _userRepository = userRepository;
            _fileRepository = fileRepository;
        }

        /// <summary>
        /// 检查用户是否有足够的配额上传文件
        /// </summary>
        /// <returns>true=配额充足，false=配额不足</returns>
        public async Task<bool> HasSufficientQuotaAsync(Guid userId, long requiredBytes)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException($"用户不存在：{userId}");

            return user.HasEnoughSpace(requiredBytes);
        }

        /// <summary>
        /// 获取用户剩余配额（字节）
        /// </summary>
        public async Task<long> GetRemainingQuotaAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException($"用户不存在：{userId}");

            return user.GetRemainingSpace();
        }

        /// <summary>
        /// 获取用户配额使用详情
        /// </summary>
        public async Task<QuotaInfo> GetQuotaInfoAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException($"用户不存在：{userId}");

            return new QuotaInfo(
                TotalQuota: user.TotalQuota,
                UsedSpace: user.UsedSpace,
                RemainingSpace: user.GetRemainingSpace(),
                UsagePercentage: user.GetSpaceUsagePercentage(),
                VipLevel: user.GetEffectiveVipLevel(),
                MaxSingleFileSize: GetMaxSingleFileSize(user.GetEffectiveVipLevel())
            );
        }

        /// <summary>
        /// 根据VIP等级获取单文件大小上限（字节）
        /// </summary>
        public static long GetMaxSingleFileSize(int vipLevel)
        {
            return vipLevel switch
            {
                0 => 100L * 1024 * 1024,           // 普通用户：100MB
                1 => 500L * 1024 * 1024,           // VIP1：500MB
                2 => 1024L * 1024 * 1024,          // VIP2：1GB
                3 => 5L * 1024 * 1024 * 1024,      // VIP3：5GB
                4 => 10L * 1024 * 1024 * 1024,     // VIP4：10GB
                5 => long.MaxValue,                 // VIP5：不限大小
                _ => 100L * 1024 * 1024             // 默认：100MB
            };
        }

        /// <summary>
        /// 同步用户已用空间（从文件仓储重新计算）
        /// </summary>
        public async Task RecalculateUsedSpaceAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException($"用户不存在：{userId}");

            var actualUsedSpace = await _fileRepository.GetTotalSizeByOwnerAsync(userId);
            var currentUsed = user.UsedSpace;

            if (actualUsedSpace > currentUsed)
            {
                user.IncreaseUsedSpace(actualUsedSpace - currentUsed);
            }
            else if (actualUsedSpace < currentUsed)
            {
                user.DecreaseUsedSpace(currentUsed - actualUsedSpace);
            }

            await _userRepository.UpdateAsync(user);
        }
    }

    /// <summary>
    /// 配额信息
    /// </summary>
    public record QuotaInfo(
        long TotalQuota,
        long UsedSpace,
        long RemainingSpace,
        double UsagePercentage,
        int VipLevel,
        long MaxSingleFileSize
    );
}
