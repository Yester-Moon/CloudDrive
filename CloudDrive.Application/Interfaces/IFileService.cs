using CloudDrive.Application.Commands;
using CloudDrive.Application.Dtos;
using CloudDrive.Application.Queries;

namespace CloudDrive.Application.Interfaces
{
    /// <summary>
    /// 文件服务接口
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        Task<FileUploadResultDto> UploadFileAsync(UploadFileCommand command);

        /// <summary>
        /// 秒传检测（根据哈希检查文件是否已存在）
        /// </summary>
        Task<FileUploadResultDto> TryInstantUploadAsync(Guid ownerId, string fileHash, string fileName, string mimeType, Guid? parentFolderId = null);

        /// <summary>
        /// 下载文件（获取文件流）
        /// </summary>
        Task<(Stream FileStream, string FileName, string MimeType)> DownloadFileAsync(Guid fileId, Guid userId);

        /// <summary>
        /// 获取文件信息
        /// </summary>
        Task<FileInfoDto?> GetFileInfoAsync(Guid fileId, Guid userId);

        /// <summary>
        /// 获取文件列表（分页）
        /// </summary>
        Task<FileListDto> GetFileListAsync(FileListQuery query);

        /// <summary>
        /// 搜索文件
        /// </summary>
        Task<FileListDto> SearchFilesAsync(FileSearchQuery query);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        Task<FileInfoDto> CreateFolderAsync(CreateFolderCommand command);

        /// <summary>
        /// 删除文件（软删除）
        /// </summary>
        Task DeleteFileAsync(DeleteFileCommand command);

        /// <summary>
        /// 重命名文件
        /// </summary>
        Task<FileInfoDto> RenameFileAsync(RenameFileCommand command);

        /// <summary>
        /// 移动文件
        /// </summary>
        Task MoveFileAsync(MoveFileCommand command);

        /// <summary>
        /// 复制文件
        /// </summary>
        Task<FileInfoDto> CopyFileAsync(Guid fileId, Guid userId, Guid? targetFolderId);

        /// <summary>
        /// 批量删除文件（软删除）
        /// </summary>
        Task<int> BatchDeleteAsync(BatchDeleteCommand command);

        /// <summary>
        /// 批量移动文件
        /// </summary>
        Task<int> BatchMoveAsync(BatchMoveCommand command);

        /// <summary>
        /// 获取回收站文件列表（分页）
        /// </summary>
        Task<FileListDto> GetTrashListAsync(Guid userId, int pageIndex, int pageSize);

        /// <summary>
        /// 从回收站恢复文件
        /// </summary>
        Task<FileInfoDto> RestoreFileAsync(RestoreFileCommand command);

        /// <summary>
        /// 清空回收站
        /// </summary>
        Task<int> EmptyTrashAsync(Guid userId);

        #region 分片上传

        /// <summary>
        /// 初始化分片上传会话
        /// </summary>
        Task<ChunkUploadSessionDto> InitChunkUploadAsync(InitChunkUploadCommand command);

        /// <summary>
        /// 上传单个分片
        /// </summary>
        Task<ChunkUploadSessionDto> UploadChunkAsync(UploadChunkCommand command);

        /// <summary>
        /// 完成分片上传（合并分片并创建文件）
        /// </summary>
        Task<FileUploadResultDto> CompleteChunkUploadAsync(CompleteChunkUploadCommand command);

        #endregion

        #region 文件预览

        /// <summary>
        /// 获取文件预览（图片/音视频直接返回流，文本返回内容，其他返回预览信息）
        /// </summary>
        Task<FilePreviewResult> GetFilePreviewAsync(Guid fileId, Guid userId);

        #endregion
    }

    /// <summary>
    /// 文件预览结果
    /// </summary>
    public class FilePreviewResult
    {
        /// <summary>
        /// 预览类型：Stream / Text / Unsupported
        /// </summary>
        public string PreviewType { get; set; } = string.Empty;

        /// <summary>
        /// 文件流（图片/视频/音频/PDF场景）
        /// </summary>
        public Stream? FileStream { get; set; }

        /// <summary>
        /// MIME类型
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 文本内容（文本文件预览场景）
        /// </summary>
        public string? TextContent { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Extension { get; set; } = string.Empty;

        public static FilePreviewResult StreamPreview(Stream stream, string mimeType, string fileName, string extension)
            => new() { PreviewType = "Stream", FileStream = stream, MimeType = mimeType, FileName = fileName, Extension = extension };

        public static FilePreviewResult TextPreview(string content, string mimeType, string fileName, string extension)
            => new() { PreviewType = "Text", TextContent = content, MimeType = mimeType, FileName = fileName, Extension = extension };

        public static FilePreviewResult Unsupported(string fileName, string extension)
            => new() { PreviewType = "Unsupported", FileName = fileName, Extension = extension };
    }
}
