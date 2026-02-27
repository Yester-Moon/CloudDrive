using CloudDrive.Application.Commands;
using CloudDrive.Application.Interfaces;
using CloudDrive.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CloudDrive.WebApi.Controllers
{
    /// <summary>
    /// 分享管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ShareController : ControllerBase
    {
        private readonly IShareService _shareService;
        private readonly ILogger<ShareController> _logger;

        public ShareController(IShareService shareService, ILogger<ShareController> logger)
        {
            _shareService = shareService;
            _logger = logger;
        }

        /// <summary>
        /// 创建分享链接
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateShare([FromBody] CreateShareRequest request)
        {
            var userId = GetCurrentUserId();

            var command = new CreateShareLinkCommand
            {
                CreatorId = userId,
                FileItemId = request.FileItemId,
                Title = request.Title,
                AccessPassword = request.AccessPassword,
                ExpirationTime = request.ExpirationTime,
                MaxDownloadCount = request.MaxDownloadCount,
                AllowDownload = request.AllowDownload
            };

            var result = await _shareService.CreateShareLinkAsync(command);
            return Ok(ApiResponse.Ok(result, "分享创建成功"));
        }

        /// <summary>
        /// 根据分享码获取分享信息（公开访问）
        /// </summary>
        [HttpGet("{code}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> GetShareByCode(string code)
        {
            var result = await _shareService.GetShareByCodeAsync(code);
            if (result == null)
                return NotFound(ApiResponse.Fail("分享链接不存在", 404));

            return Ok(ApiResponse.Ok(result));
        }

        /// <summary>
        /// 验证分享密码并获取文件信息
        /// </summary>
        [HttpPost("{code}/verify")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> VerifyAndGetFile(string code, [FromBody] VerifyShareRequest request)
        {
            var fileInfo = await _shareService.GetSharedFileAsync(code, request.Password);
            if (fileInfo == null)
                return NotFound(ApiResponse.Fail("文件不存在", 404));

            return Ok(ApiResponse.Ok(fileInfo, "验证成功"));
        }

        /// <summary>
        /// 通过分享链接下载文件
        /// </summary>
        [HttpGet("{code}/download")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadSharedFile(string code, [FromQuery] string? password = null)
        {
            var (stream, fileName, mimeType) = await _shareService.DownloadSharedFileAsync(code, password);
            return File(stream, mimeType, fileName);
        }

        /// <summary>
        /// 取消分享
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> CancelShare(Guid id)
        {
            var userId = GetCurrentUserId();
            await _shareService.CancelShareAsync(new CancelShareCommand
            {
                ShareLinkId = id,
                UserId = userId
            });
            return Ok(ApiResponse.Ok(message: "分享已取消"));
        }

        /// <summary>
        /// 获取我的分享列表
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetMyShares()
        {
            var userId = GetCurrentUserId();
            var result = await _shareService.GetUserSharesAsync(userId);
            return Ok(ApiResponse.Ok(result));
        }

        /// <summary>
        /// 获取分享统计
        /// </summary>
        [Authorize]
        [HttpGet("{id}/stats")]
        public async Task<ActionResult<ApiResponse>> GetShareStats(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _shareService.GetShareStatsAsync(id, userId);
            if (result == null)
                return NotFound(ApiResponse.Fail("分享链接不存在", 404));

            return Ok(ApiResponse.Ok(result));
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }

    #region Request Models

    public class CreateShareRequest
    {
        /// <summary>
        /// 被分享的文件ID
        /// </summary>
        public Guid FileItemId { get; set; }

        /// <summary>
        /// 分享标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 访问密码
        /// </summary>
        public string? AccessPassword { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// 最大下载次数
        /// </summary>
        public int? MaxDownloadCount { get; set; }

        /// <summary>
        /// 是否允许下载
        /// </summary>
        public bool AllowDownload { get; set; } = true;
    }

    public class VerifyShareRequest
    {
        /// <summary>
        /// 访问密码
        /// </summary>
        public string? Password { get; set; }
    }

    #endregion
}
