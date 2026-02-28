using CloudDrive.Application.Commands;
using CloudDrive.Common.Validators;
using FluentValidation;

namespace CloudDrive.Application.Validators
{
    /// <summary>
    /// 上传文件命令验证器
    /// </summary>
    public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
    {
        public UploadFileCommandValidator()
        {
            RuleFor(x => x.OwnerId).MustBeValidGuid();
            RuleFor(x => x.FileName)
                .MustBeValidFileName()
                .MustNotBeBlockedExtension();
            RuleFor(x => x.FileSize).MustBeWithinFileSize();
            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("MIME类型不能为空");
        }
    }

    /// <summary>
    /// 创建文件夹命令验证器
    /// </summary>
    public class CreateFolderCommandValidator : AbstractValidator<CreateFolderCommand>
    {
        public CreateFolderCommandValidator()
        {
            RuleFor(x => x.OwnerId).MustBeValidGuid();
            RuleFor(x => x.FolderName)
                .NotEmpty().WithMessage("文件夹名称不能为空")
                .MaximumLength(255).WithMessage("文件夹名称长度不能超过255个字符");
        }
    }

    /// <summary>
    /// 重命名文件命令验证器
    /// </summary>
    public class RenameFileCommandValidator : AbstractValidator<RenameFileCommand>
    {
        public RenameFileCommandValidator()
        {
            RuleFor(x => x.FileId).MustBeValidGuid();
            RuleFor(x => x.UserId).MustBeValidGuid();
            RuleFor(x => x.NewName).MustBeValidFileName();
        }
    }

    /// <summary>
    /// 移动文件命令验证器
    /// </summary>
    public class MoveFileCommandValidator : AbstractValidator<MoveFileCommand>
    {
        public MoveFileCommandValidator()
        {
            RuleFor(x => x.FileId).MustBeValidGuid();
            RuleFor(x => x.UserId).MustBeValidGuid();
        }
    }

    /// <summary>
    /// 删除文件命令验证器
    /// </summary>
    public class DeleteFileCommandValidator : AbstractValidator<DeleteFileCommand>
    {
        public DeleteFileCommandValidator()
        {
            RuleFor(x => x.FileId).MustBeValidGuid();
            RuleFor(x => x.UserId).MustBeValidGuid();
        }
    }

    /// <summary>
    /// 批量删除文件命令验证器
    /// </summary>
    public class BatchDeleteCommandValidator : AbstractValidator<BatchDeleteCommand>
    {
        public BatchDeleteCommandValidator()
        {
            RuleFor(x => x.UserId).MustBeValidGuid();
            RuleFor(x => x.FileIds)
                .NotEmpty().WithMessage("文件ID列表不能为空");
            RuleForEach(x => x.FileIds).MustBeValidGuid();
        }
    }

    /// <summary>
    /// 批量移动文件命令验证器
    /// </summary>
    public class BatchMoveCommandValidator : AbstractValidator<BatchMoveCommand>
    {
        public BatchMoveCommandValidator()
        {
            RuleFor(x => x.UserId).MustBeValidGuid();
            RuleFor(x => x.FileIds)
                .NotEmpty().WithMessage("文件ID列表不能为空");
            RuleForEach(x => x.FileIds).MustBeValidGuid();
        }
    }

    /// <summary>
    /// 恢复文件命令验证器
    /// </summary>
    public class RestoreFileCommandValidator : AbstractValidator<RestoreFileCommand>
    {
        public RestoreFileCommandValidator()
        {
            RuleFor(x => x.FileId).MustBeValidGuid();
            RuleFor(x => x.UserId).MustBeValidGuid();
        }
    }

    /// <summary>
    /// 创建分享链接命令验证器
    /// </summary>
    public class CreateShareLinkCommandValidator : AbstractValidator<CreateShareLinkCommand>
    {
        public CreateShareLinkCommandValidator()
        {
            RuleFor(x => x.CreatorId).MustBeValidGuid();
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
    /// 取消分享命令验证器
    /// </summary>
    public class CancelShareCommandValidator : AbstractValidator<CancelShareCommand>
    {
        public CancelShareCommandValidator()
        {
            RuleFor(x => x.ShareLinkId).MustBeValidGuid();
            RuleFor(x => x.UserId).MustBeValidGuid();
        }
    }
}
