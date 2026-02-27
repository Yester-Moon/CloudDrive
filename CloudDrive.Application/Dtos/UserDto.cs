namespace CloudDrive.Application.Dtos
{
    /// <summary>
    /// 用户信息DTO
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// VIP等级
        /// </summary>
        public int VipLevel { get; set; }

        /// <summary>
        /// VIP过期时间
        /// </summary>
        public DateTime? VipExpirationTime { get; set; }

        /// <summary>
        /// 是否VIP有效
        /// </summary>
        public bool IsVipActive { get; set; }

        /// <summary>
        /// 账户创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 配额信息
        /// </summary>
        public QuotaDto? Quota { get; set; }
    }
}
