using CloudDrive.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CloudDrive.Domain.Entities
{
    /// <summary>
    /// 用户实体（继承自 Identity）
    /// </summary>
    public class User : IdentityUser<Guid>
    {
        /// <summary>
        /// 昵称/显示名称
        /// </summary>
        public string? DisplayName { get; private set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string? AvatarUrl { get; private set; }

        /// <summary>
        /// VIP等级（0=普通用户，1-5=VIP等级）
        /// </summary>
        public int VipLevel { get; private set; }

        /// <summary>
        /// VIP过期时间（null表示非VIP或永久）
        /// </summary>
        public DateTime? VipExpirationTime { get; private set; }

        /// <summary>
        /// 总存储配额（字节）
        /// </summary>
        public long TotalQuota { get; private set; }

        /// <summary>
        /// 已使用空间（字节）
        /// </summary>
        public long UsedSpace { get; private set; }

        /// <summary>
        /// 账户创建时间
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginAt { get; private set; }

        /// <summary>
        /// 是否已激活
        /// </summary>
        public bool IsActivated { get; private set; }

        /// <summary>
        /// 账户是否被封禁
        /// </summary>
        public bool IsBanned { get; private set; }

        /// <summary>
        /// 封禁原因
        /// </summary>
        public string? BanReason { get; private set; }

        /// <summary>
        /// 用户的文件集合
        /// </summary>
        public ICollection<FileItem>? FileItems { get; private set; }

        /// <summary>
        /// 用户的分享链接集合
        /// </summary>
        public ICollection<ShareLink>? ShareLinks { get; private set; }

        // 领域事件列表
        private readonly List<INotification> _domainEvents = new();

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// 默认构造函数（EF Core需要）
        /// </summary>
        public User()
        {
            Id = Guid.NewGuid();
            VipLevel = 0;
            TotalQuota = GetDefaultQuota(0); // 普通用户默认配额
            UsedSpace = 0;
            CreatedAt = DateTime.Now;
            IsActivated = false;
            IsBanned = false;
        }

        /// <summary>
        /// 创建新用户（工厂方法）
        /// </summary>
        public static User Create(string userName, string email)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("用户名不能为空", nameof(userName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("邮箱不能为空", nameof(email));

            var user = new User
            {
                UserName = userName,
                Email = email,
                DisplayName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                NormalizedEmail = email.ToUpperInvariant()
            };

            return user;
        }

        /// <summary>
        /// 激活账户
        /// </summary>
        public void Activate()
        {
            IsActivated = true;
        }

        /// <summary>
        /// 更新显示名称
        /// </summary>
        public void UpdateDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("显示名称不能为空");

            DisplayName = displayName;
        }

        /// <summary>
        /// 更新头像
        /// </summary>
        public void UpdateAvatar(string avatarUrl)
        {
            AvatarUrl = avatarUrl;
        }

        /// <summary>
        /// 记录登录
        /// </summary>
        public void RecordLogin()
        {
            LastLoginAt = DateTime.Now;
        }

        /// <summary>
        /// 增加已使用空间
        /// </summary>
        public void IncreaseUsedSpace(long bytes)
        {
            if (bytes <= 0)
                throw new ArgumentException("字节数必须大于0", nameof(bytes));

            var newUsedSpace = UsedSpace + bytes;

            // 检查是否超配额
            if (newUsedSpace > TotalQuota)
            {
                AddDomainEvent(new StorageQuotaExeededEvent(Id, UsedSpace, TotalQuota, bytes));
                throw new InvalidOperationException($"存储空间不足。已用：{UsedSpace}，配额：{TotalQuota}，需要：{bytes}");
            }

            UsedSpace = newUsedSpace;
            AddDomainEvent(new QuotaChangedEvent(Id, UsedSpace, TotalQuota));
        }

        /// <summary>
        /// 减少已使用空间
        /// </summary>
        public void DecreaseUsedSpace(long bytes)
        {
            if (bytes <= 0)
                throw new ArgumentException("字节数必须大于0", nameof(bytes));

            UsedSpace = Math.Max(0, UsedSpace - bytes);
            AddDomainEvent(new QuotaChangedEvent(Id, UsedSpace, TotalQuota));
        }

        /// <summary>
        /// 同步已使用空间（后台任务修正漂移，直接设置为实际值）
        /// </summary>
        public void SyncUsedSpace(long actualBytes)
        {
            UsedSpace = Math.Max(0, actualBytes);
        }

        /// <summary>
        /// 升级VIP
        /// </summary>
        public void UpgradeVip(int newVipLevel, DateTime? expirationTime = null)
        {
            if (newVipLevel < 0 || newVipLevel > 5)
                throw new ArgumentException("VIP等级必须在0-5之间", nameof(newVipLevel));

            var oldQuota = TotalQuota;
            VipLevel = newVipLevel;
            VipExpirationTime = expirationTime;
            TotalQuota = GetDefaultQuota(newVipLevel);

            // 触发配额变更事件
            AddDomainEvent(new QuotaChangedEvent(Id, UsedSpace, TotalQuota));
        }

        /// <summary>
        /// 检查VIP是否有效
        /// </summary>
        public bool IsVipActive()
        {
            if (VipLevel == 0) return false;
            if (!VipExpirationTime.HasValue) return true; // 永久VIP
            return VipExpirationTime.Value > DateTime.Now;
        }

        /// <summary>
        /// 获取当前有效的VIP等级
        /// </summary>
        public int GetEffectiveVipLevel()
        {
            return IsVipActive() ? VipLevel : 0;
        }

        /// <summary>
        /// 封禁用户
        /// </summary>
        public void Ban(string reason)
        {
            IsBanned = true;
            BanReason = reason;
        }

        /// <summary>
        /// 解除封禁
        /// </summary>
        public void Unban()
        {
            IsBanned = false;
            BanReason = null;
        }

        /// <summary>
        /// 获取剩余空间
        /// </summary>
        public long GetRemainingSpace()
        {
            return Math.Max(0, TotalQuota - UsedSpace);
        }

        /// <summary>
        /// 获取空间使用率（百分比）
        /// </summary>
        public double GetSpaceUsagePercentage()
        {
            if (TotalQuota == 0) return 0;
            return (double)UsedSpace / TotalQuota * 100;
        }

        /// <summary>
        /// 检查是否有足够空间
        /// </summary>
        public bool HasEnoughSpace(long requiredBytes)
        {
            return GetRemainingSpace() >= requiredBytes;
        }

        /// <summary>
        /// 根据VIP等级获取默认配额
        /// </summary>
        private static long GetDefaultQuota(int vipLevel)
        {
            return vipLevel switch
            {
                0 => 10L * 1024 * 1024 * 1024,      // 普通用户：10GB
                1 => 50L * 1024 * 1024 * 1024,      // VIP1：50GB
                2 => 100L * 1024 * 1024 * 1024,     // VIP2：100GB
                3 => 500L * 1024 * 1024 * 1024,     // VIP3：500GB
                4 => 1024L * 1024 * 1024 * 1024,    // VIP4：1TB
                5 => 5120L * 1024 * 1024 * 1024,    // VIP5：5TB
                _ => 10L * 1024 * 1024 * 1024       // 默认：10GB
            };
        }

        /// <summary>
        /// 获取格式化的配额信息
        /// </summary>
        public string GetFormattedQuota()
        {
            return $"{FormatBytes(UsedSpace)} / {FormatBytes(TotalQuota)}";
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
