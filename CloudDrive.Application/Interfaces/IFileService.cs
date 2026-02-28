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
    }
}
