using CloudDrive.Application.Commands;
using CloudDrive.Application.Dtos;

namespace CloudDrive.Application.Interfaces
{
    /// <summary>
    /// 分享服务接口
    /// </summary>
    public interface IShareService
    {
        /// <summary>
        /// 创建分享链接
        /// </summary>
        Task<ShareLinkDto> CreateShareLinkAsync(CreateShareLinkCommand command);

        /// <summary>
        /// 根据分享码获取分享信息（公开访问）
        /// </summary>
        Task<ShareLinkDto?> GetShareByCodeAsync(string shareCode);

        /// <summary>
        /// 获取分享文件（验证密码后获取文件信息）
        /// </summary>
        Task<FileInfoDto?> GetSharedFileAsync(string shareCode, string? password = null);

        /// <summary>
        /// 通过分享链接下载文件
        /// </summary>
        Task<(Stream FileStream, string FileName, string MimeType)> DownloadSharedFileAsync(string shareCode, string? password = null);

        /// <summary>
        /// 获取用户创建的所有分享链接
        /// </summary>
        Task<List<ShareLinkDto>> GetUserSharesAsync(Guid userId);

        /// <summary>
        /// 取消分享
        /// </summary>
        Task CancelShareAsync(CancelShareCommand command);

        /// <summary>
        /// 获取分享统计（下载次数、查看次数）
        /// </summary>
        Task<ShareLinkDto?> GetShareStatsAsync(Guid shareLinkId, Guid userId);
    }
}
