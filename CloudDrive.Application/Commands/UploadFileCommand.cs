namespace CloudDrive.Application.Commands
{
    /// <summary>
    /// 上传文件命令
    /// </summary>
    public class UploadFileCommand
    {
        /// <summary>
        /// 上传者ID
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 文件流
        /// </summary>
        public Stream FileStream { get; set; } = Stream.Null;

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// MIME类型
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// 文件哈希（客户端预计算，用于秒传检测）
        /// </summary>
        public string? FileHash { get; set; }

        /// <summary>
        /// 目标文件夹ID（null为根目录）
        /// </summary>
        public Guid? ParentFolderId { get; set; }
    }

    /// <summary>
    /// 创建文件夹命令
    /// </summary>
    public class CreateFolderCommand
    {
        /// <summary>
        /// 所有者ID
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string FolderName { get; set; } = string.Empty;

        /// <summary>
        /// 父文件夹ID
        /// </summary>
        public Guid? ParentFolderId { get; set; }
    }

    /// <summary>
    /// 重命名文件命令
    /// </summary>
    public class RenameFileCommand
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 新文件名
        /// </summary>
        public string NewName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 移动文件命令
    /// </summary>
    public class MoveFileCommand
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 目标文件夹ID
        /// </summary>
        public Guid? TargetFolderId { get; set; }
    }

    /// <summary>
    /// 删除文件命令
    /// </summary>
    public class DeleteFileCommand
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
