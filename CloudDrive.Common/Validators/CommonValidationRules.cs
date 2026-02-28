using CloudDrive.Common.Constants;
using FluentValidation;

namespace CloudDrive.Common.Validators
{
    /// <summary>
    /// 通用 FluentValidation 验证规则扩展
    /// </summary>
    public static class CommonValidationRules
    {
        /// <summary>
        /// 验证文件名是否合法
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustBeValidFileName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("文件名不能为空")
                .MaximumLength(255).WithMessage("文件名长度不能超过255个字符")
                .Must(name =>
                {
                    var invalidChars = Path.GetInvalidFileNameChars();
                    return !name.Any(c => invalidChars.Contains(c));
                }).WithMessage("文件名包含非法字符");
        }

        /// <summary>
        /// 验证文件扩展名不在黑名单中
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustNotBeBlockedExtension<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(fileName =>
                {
                    var ext = Path.GetExtension(fileName);
                    return !FileTypeConstants.IsBlocked(ext);
                }).WithMessage("不允许上传该类型的文件");
        }

        /// <summary>
        /// 验证文件大小在限制范围内
        /// </summary>
        public static IRuleBuilderOptions<T, long> MustBeWithinFileSize<T>(
            this IRuleBuilder<T, long> ruleBuilder,
            long maxSize = ConfigConstants.DefaultMaxFileSize)
        {
            return ruleBuilder
                .GreaterThan(0).WithMessage("文件大小必须大于0")
                .LessThanOrEqualTo(maxSize).WithMessage($"文件大小不能超过 {maxSize / (1024 * 1024)} MB");
        }

        /// <summary>
        /// 验证分页页码
        /// </summary>
        public static IRuleBuilderOptions<T, int> MustBeValidPageIndex<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(1).WithMessage("页码必须大于等于1");
        }

        /// <summary>
        /// 验证分页大小
        /// </summary>
        public static IRuleBuilderOptions<T, int> MustBeValidPageSize<T>(
            this IRuleBuilder<T, int> ruleBuilder,
            int maxPageSize = ConfigConstants.MaxPageSize)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(1).WithMessage("每页数量必须大于等于1")
                .LessThanOrEqualTo(maxPageSize).WithMessage($"每页数量不能超过 {maxPageSize}");
        }

        /// <summary>
        /// 验证邮箱格式
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustBeValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("邮箱不能为空")
                .EmailAddress().WithMessage("邮箱格式不正确")
                .MaximumLength(256).WithMessage("邮箱长度不能超过256个字符");
        }

        /// <summary>
        /// 验证用户名格式
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustBeValidUserName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("用户名不能为空")
                .MinimumLength(2).WithMessage("用户名长度不能少于2个字符")
                .MaximumLength(50).WithMessage("用户名长度不能超过50个字符")
                .Matches(@"^[a-zA-Z0-9_\u4e00-\u9fa5]+$").WithMessage("用户名只能包含字母、数字、下划线和中文");
        }

        /// <summary>
        /// 验证密码强度
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustBeValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("密码不能为空")
                .MinimumLength(6).WithMessage("密码长度不能少于6个字符")
                .MaximumLength(128).WithMessage("密码长度不能超过128个字符");
        }

        /// <summary>
        /// 验证 GUID 非空
        /// </summary>
        public static IRuleBuilderOptions<T, Guid> MustBeValidGuid<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        {
            return ruleBuilder
                .NotEqual(Guid.Empty).WithMessage("ID 不能为空");
        }

        /// <summary>
        /// 验证分享密码格式
        /// </summary>
        public static IRuleBuilderOptions<T, string?> MustBeValidSharePassword<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(20).WithMessage("分享密码长度不能超过20个字符")
                .Matches(@"^[a-zA-Z0-9]*$").When(x => x != null).WithMessage("分享密码只能包含字母和数字");
        }
    }
}
