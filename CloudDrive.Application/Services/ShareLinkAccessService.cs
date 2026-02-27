using CloudDrive.Domain.Entities;

namespace CloudDrive.Application.Services
{
    /// <summary>
    /// 分享链接访问领域服务
    /// </summary>
    public class ShareLinkAccessService
    {
        /// <summary>
        /// 验证分享链接的访问权限（完整校验）
        /// </summary>
        /// <returns>访问验证结果</returns>
        public ShareLinkAccessResult ValidateAccess(ShareLink shareLink, string? password = null)
        {
            // 1. 检查分享链接是否有效（过期、取消、删除、下载次数）
            if (!shareLink.IsValid())
                return ShareLinkAccessResult.Fail($"分享链接已失效：{shareLink.GetStatusDescription()}");

            // 2. 验证访问密码
            if (!shareLink.VerifyPassword(password ?? string.Empty))
                return ShareLinkAccessResult.Fail("访问密码错误");

            return ShareLinkAccessResult.Success();
        }

        /// <summary>
        /// 验证下载权限
        /// </summary>
        public ShareLinkAccessResult ValidateDownload(ShareLink shareLink, string? password = null)
        {
            // 先做基础访问验证
            var accessResult = ValidateAccess(shareLink, password);
            if (!accessResult.IsAllowed)
                return accessResult;

            // 检查是否允许下载
            if (!shareLink.AllowDownload)
                return ShareLinkAccessResult.Fail("此分享仅支持预览，不允许下载");

            // 检查下载次数是否已达上限
            if (shareLink.MaxDownloadCount.HasValue &&
                shareLink.CurrentDownloadCount >= shareLink.MaxDownloadCount.Value)
                return ShareLinkAccessResult.Fail("下载次数已达上限");

            return ShareLinkAccessResult.Success();
        }

        /// <summary>
        /// 验证分享链接的所有者权限（仅创建者可操作）
        /// </summary>
        public ShareLinkAccessResult ValidateOwnership(ShareLink shareLink, Guid userId)
        {
            if (shareLink.CreatorId != userId)
                return ShareLinkAccessResult.Fail("无权操作此分享链接");

            return ShareLinkAccessResult.Success();
        }

        /// <summary>
        /// 验证是否可以创建分享（文件所有者才能分享）
        /// </summary>
        public ShareLinkAccessResult ValidateCreateShare(FileItem fileItem, Guid userId)
        {
            if (fileItem.OwnerId != userId)
                return ShareLinkAccessResult.Fail("只能分享自己的文件");

            if (fileItem.IsDeleted)
                return ShareLinkAccessResult.Fail("文件已被删除，无法创建分享");

            if (fileItem.IsFolder)
                return ShareLinkAccessResult.Fail("暂不支持分享文件夹");

            return ShareLinkAccessResult.Success();
        }
    }

    /// <summary>
    /// 分享链接访问验证结果
    /// </summary>
    public class ShareLinkAccessResult
    {
        /// <summary>
        /// 是否允许访问
        /// </summary>
        public bool IsAllowed { get; private set; }

        /// <summary>
        /// 错误/提示消息
        /// </summary>
        public string? Message { get; private set; }

        public static ShareLinkAccessResult Success()
            => new() { IsAllowed = true };

        public static ShareLinkAccessResult Fail(string message)
            => new() { IsAllowed = false, Message = message };
    }
}
