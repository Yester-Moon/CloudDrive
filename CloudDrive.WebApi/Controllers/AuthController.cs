using CloudDrive.Application.Interfaces;
using CloudDrive.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudDrive.WebApi.Controllers
{
    /// <summary>
    /// 认证授权控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequest request)
        {
            var user = await _userService.RegisterAsync(request.UserName, request.Email, request.Password);
            return Ok(ApiResponse.Ok(user, "注册成功"));
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequest request)
        {
            var (user, token) = await _userService.LoginAsync(request.UserName, request.Password);
            return Ok(ApiResponse.Ok(new { user, token }, "登录成功"));
        }

        /// <summary>
        /// 刷新Token（需要登录）
        /// </summary>
        [Authorize]
        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse>> Refresh()
        {
            // 简化实现：根据当前Token中的用户ID重新生成用户信息
            // 生产环境应使用 Refresh Token 机制
            var userId = GetCurrentUserId();
            var userInfo = await _userService.GetUserInfoAsync(userId);
            if (userInfo == null)
                return NotFound(ApiResponse.Fail("用户不存在", 404));

            return Ok(ApiResponse.Ok(new { user = userInfo }, "Token刷新成功"));
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public ActionResult<ApiResponse> Logout()
        {
            // JWT是无状态的，登出由客户端删除Token实现
            // 如需服务端失效，可将Token加入黑名单（Redis）
            return Ok(ApiResponse.Ok(message: "登出成功"));
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }

    /// <summary>
    /// 注册请求
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 登录请求
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
