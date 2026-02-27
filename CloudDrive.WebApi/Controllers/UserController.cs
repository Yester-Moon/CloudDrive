using CloudDrive.Application.Interfaces;
using CloudDrive.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CloudDrive.WebApi.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly IShareService _shareService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IFileService fileService,
            IShareService shareService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _fileService = fileService;
            _shareService = shareService;
            _logger = logger;
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse>> GetProfile()
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetUserInfoAsync(userId);
            if (user == null)
                return NotFound(ApiResponse.Fail("用户不存在", 404));

            return Ok(ApiResponse.Ok(user));
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();

            if (!string.IsNullOrWhiteSpace(request.DisplayName))
            {
                await _userService.UpdateDisplayNameAsync(userId, request.DisplayName);
            }

            if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
            {
                await _userService.UpdateAvatarAsync(userId, request.AvatarUrl);
            }

            var user = await _userService.GetUserInfoAsync(userId);
            return Ok(ApiResponse.Ok(user, "更新成功"));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        [HttpPost("password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();
            await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            return Ok(ApiResponse.Ok(message: "密码修改成功"));
        }

        /// <summary>
        /// 获取配额信息
        /// </summary>
        [HttpGet("quota")]
        public async Task<ActionResult<ApiResponse>> GetQuota()
        {
            var userId = GetCurrentUserId();
            var quota = await _userService.GetQuotaAsync(userId);
            return Ok(ApiResponse.Ok(quota));
        }

        /// <summary>
        /// 获取用户统计信息
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse>> GetStatistics()
        {
            var userId = GetCurrentUserId();

            // 获取配额信息
            var quota = await _userService.GetQuotaAsync(userId);

            // 获取文件统计（获取全部文件）
            var files = await _fileService.GetFileListAsync(new Application.Queries.FileListQuery
            {
                OwnerId = userId,
                PageIndex = 1,
                PageSize = 1
            });

            // 获取分享统计
            var shares = await _shareService.GetUserSharesAsync(userId);

            var statistics = new
            {
                Quota = quota,
                TotalFiles = files.TotalCount,
                TotalShares = shares.Count,
                ActiveShares = shares.Count(s => s.IsValid),
                TotalDownloads = shares.Sum(s => s.CurrentDownloadCount),
                TotalViews = shares.Sum(s => s.ViewCount)
            };

            return Ok(ApiResponse.Ok(statistics));
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }

    #region Request Models

    public class UpdateProfileRequest
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string? AvatarUrl { get; set; }
    }

    public class ChangePasswordRequest
    {
        /// <summary>
        /// 当前密码
        /// </summary>
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }

    #endregion
}
