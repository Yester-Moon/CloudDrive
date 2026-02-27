using CloudDrive.Common.Models;
using CloudDrive.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CloudDrive.Domain.Entities
{
    /// <summary>
    /// 分享链接实体 - 聚合根
    /// </summary>
    public record ShareLink : AggregateRootEntity
    {
        /// <summary>
        /// 分享码（短链接标识）
        /// </summary>
        public string ShareCode { get; private set; }

        /// <summary>
        /// 被分享的文件ID
        /// </summary>
        public Guid FileItemId { get; private set; }

        /// <summary>
        /// 分享创建者ID
        /// </summary>
        public Guid CreatorId { get; private set; }

        /// <summary>
        /// 分享标题
        /// </summary>
        public string? Title { get; private set; }

        /// <summary>
        /// 访问密码（可选，null表示无密码）
        /// </summary>
        public string? AccessPassword { get; private set; }

        /// <summary>
        /// 过期时间（null表示永久有效）
        /// </summary>
        public DateTime? ExpirationTime { get; private set; }

        /// <summary>
        /// 最大下载次数限制（null表示无限制）
        /// </summary>
        public int? MaxDownloadCount { get; private set; }

        /// <summary>
        /// 当前下载次数
        /// </summary>
        public int CurrentDownloadCount { get; private set; }

        /// <summary>
        /// 访问次数（查看次数）
        /// </summary>
        public int ViewCount { get; private set; }

        /// <summary>
        /// 是否允许下载（false则仅预览）
        /// </summary>
        public bool AllowDownload { get; private set; }

        /// <summary>
        /// 是否已取消
        /// </summary>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// 文件项导航属性
        /// </summary>
        public FileItem? FileItem { get; private set; }

        /// <summary>
        /// 创建者导航属性
        /// </summary>
        public User? Creator { get; private set; }

        // 私有构造函数
        private ShareLink() { }

        /// <summary>
        /// 创建分享链接（工厂方法）
        /// </summary>
        public static ShareLink Create(
            Guid fileItemId,
            Guid creatorId,
            string? title = null,
            string? accessPassword = null,
            DateTime? expirationTime = null,
            int? maxDownloadCount = null,
            bool allowDownload = true)
        {
            // 业务规则验证
            if (expirationTime.HasValue && expirationTime.Value <= DateTime.Now)
                throw new ArgumentException("过期时间必须大于当前时间", nameof(expirationTime));

            if (maxDownloadCount.HasValue && maxDownloadCount.Value <= 0)
                throw new ArgumentException("最大下载次数必须大于0", nameof(maxDownloadCount));

            var shareLink = new ShareLink
            {
                ShareCode = GenerateShareCode(),
                FileItemId = fileItemId,
                CreatorId = creatorId,
                Title = title,
                AccessPassword = accessPassword,
                ExpirationTime = expirationTime,
                MaxDownloadCount = maxDownloadCount,
                CurrentDownloadCount = 0,
                ViewCount = 0,
                AllowDownload = allowDownload,
                IsCancelled = false
            };

            // 触发分享创建事件
            shareLink.AddNotification(new FileSharedEvent(shareLink.Id, fileItemId, creatorId));

            return shareLink;
        }

        /// <summary>
        /// 生成分享码（8位随机字符串）
        /// </summary>
        private static string GenerateShareCode()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// 验证访问密码
        /// </summary>
        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(AccessPassword))
                return true; // 无密码保护

            return AccessPassword == password;
        }

        /// <summary>
        /// 检查分享是否有效
        /// </summary>
        public bool IsValid()
        {
            // 已取消
            if (IsCancelled) return false;

            // 已删除
            if (IsDeleted) return false;

            // 已过期
            if (ExpirationTime.HasValue && ExpirationTime.Value < DateTime.Now)
                return false;

            // 超过下载次数
            if (MaxDownloadCount.HasValue && CurrentDownloadCount >= MaxDownloadCount.Value)
                return false;

            return true;
        }

        /// <summary>
        /// 增加访问次数
        /// </summary>
        public void IncrementViewCount()
        {
            ViewCount++;
            NotifyModified();
        }

        /// <summary>
        /// 增加下载次数（需验证是否允许）
        /// </summary>
        public void IncrementDownloadCount()
        {
            if (!IsValid())
                throw new InvalidOperationException("分享链接已失效");

            if (!AllowDownload)
                throw new InvalidOperationException("此分享不允许下载");

            CurrentDownloadCount++;
            NotifyModified();
        }

        /// <summary>
        /// 取消分享
        /// </summary>
        public void Cancel()
        {
            if (IsCancelled)
                throw new InvalidOperationException("分享已取消");

            IsCancelled = true;
            NotifyModified();
        }

        /// <summary>
        /// 更新过期时间
        /// </summary>
        public void UpdateExpirationTime(DateTime? newExpirationTime)
        {
            if (newExpirationTime.HasValue && newExpirationTime.Value <= DateTime.Now)
                throw new ArgumentException("过期时间必须大于当前时间");

            ExpirationTime = newExpirationTime;
            NotifyModified();
        }

        /// <summary>
        /// 更新访问密码
        /// </summary>
        public void UpdatePassword(string? newPassword)
        {
            AccessPassword = newPassword;
            NotifyModified();
        }

        /// <summary>
        /// 获取剩余下载次数
        /// </summary>
        public int? GetRemainingDownloadCount()
        {
            if (!MaxDownloadCount.HasValue)
                return null; // 无限制

            return Math.Max(0, MaxDownloadCount.Value - CurrentDownloadCount);
        }

        /// <summary>
        /// 获取分享状态描述
        /// </summary>
        public string GetStatusDescription()
        {
            if (IsCancelled) return "已取消";
            if (IsDeleted) return "已删除";
            if (ExpirationTime.HasValue && ExpirationTime.Value < DateTime.Now) return "已过期";
            if (MaxDownloadCount.HasValue && CurrentDownloadCount >= MaxDownloadCount.Value) return "已达下载上限";
            return "有效";
        }
    }
}
