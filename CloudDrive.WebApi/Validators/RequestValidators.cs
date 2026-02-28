using CloudDrive.Common.Validators;
using CloudDrive.WebApi.Controllers;
using FluentValidation;

namespace CloudDrive.WebApi.Validators
{
    /// <summary>
    /// 注册请求验证器
    /// </summary>
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.UserName).MustBeValidUserName();
            RuleFor(x => x.Email).MustBeValidEmail();
            RuleFor(x => x.Password).MustBeValidPassword();
        }
    }

    /// <summary>
    /// 登录请求验证器
    /// </summary>
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("密码不能为空");
        }
    }

    /// <summary>
    /// 秒传检测请求验证器
    /// </summary>
    public class ChunkUploadRequestValidator : AbstractValidator<SecondsUploadRequest>
    {
        public ChunkUploadRequestValidator()
        {
            RuleFor(x => x.FileHash)
                .NotEmpty().WithMessage("文件哈希不能为空");
            RuleFor(x => x.FileName).MustBeValidFileName();
            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("MIME类型不能为空");
        }
    }

    /// <summary>
    /// 重命名请求验证器
    /// </summary>
    public class RenameRequestValidator : AbstractValidator<RenameRequest>
    {
        public RenameRequestValidator()
        {
            RuleFor(x => x.NewName).MustBeValidFileName();
        }
    }

    /// <summary>
    /// 创建文件夹请求验证器
    /// </summary>
    public class CreateFolderRequestValidator : AbstractValidator<CreateFolderRequest>
    {
        public CreateFolderRequestValidator()
        {
            RuleFor(x => x.FolderName)
                .NotEmpty().WithMessage("文件夹名称不能为空")
                .MaximumLength(255).WithMessage("文件夹名称长度不能超过255个字符");
        }
    }

    /// <summary>
    /// 批量删除请求验证器
    /// </summary>
    public class BatchDeleteRequestValidator : AbstractValidator<BatchDeleteRequest>
    {
        public BatchDeleteRequestValidator()
        {
            RuleFor(x => x.FileIds)
                .NotEmpty().WithMessage("文件ID列表不能为空");
            RuleForEach(x => x.FileIds).MustBeValidGuid();
        }
    }

    /// <summary>
    /// 批量移动请求验证器
    /// </summary>
    public class BatchMoveRequestValidator : AbstractValidator<BatchMoveRequest>
    {
        public BatchMoveRequestValidator()
        {
            RuleFor(x => x.FileIds)
                .NotEmpty().WithMessage("文件ID列表不能为空");
            RuleForEach(x => x.FileIds).MustBeValidGuid();
        }
    }

    /// <summary>
    /// 创建分享请求验证器
    /// </summary>
    public class CreateShareRequestValidator : AbstractValidator<CreateShareRequest>
    {
        public CreateShareRequestValidator()
        {
            RuleFor(x => x.FileItemId).MustBeValidGuid();
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("分享标题长度不能超过200个字符")
                .When(x => x.Title != null);
            RuleFor(x => x.AccessPassword).MustBeValidSharePassword();
            RuleFor(x => x.ExpirationTime)
                .GreaterThan(DateTime.Now).WithMessage("过期时间必须大于当前时间")
                .When(x => x.ExpirationTime.HasValue);
            RuleFor(x => x.MaxDownloadCount)
                .GreaterThan(0).WithMessage("最大下载次数必须大于0")
                .When(x => x.MaxDownloadCount.HasValue);
        }
    }

    /// <summary>
    /// 修改密码请求验证器
    /// </summary>
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("当前密码不能为空");
            RuleFor(x => x.NewPassword).MustBeValidPassword();
        }
    }
}
