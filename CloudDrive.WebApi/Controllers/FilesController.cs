using CloudDrive.Application.Commands;
using CloudDrive.Application.Interfaces;
using CloudDrive.Application.Queries;
using CloudDrive.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CloudDrive.WebApi.Controllers
{
    /// <summary>
    /// 文件管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FilesController> _logger;

        public FilesController(IFileService fileService, ILogger<FilesController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(10L * 1024 * 1024 * 1024)] // 10GB
        public async Task<ActionResult<ApiResponse>> Upload(
            IFormFile file,
            [FromForm] Guid? parentFolderId = null,
            [FromForm] string? fileHash = null)
        {
            var userId = GetCurrentUserId();

            var command = new UploadFileCommand
            {
                OwnerId = userId,
                FileName = file.FileName,
                FileStream = file.OpenReadStream(),
                FileSize = file.Length,
                MimeType = file.ContentType,
                FileHash = fileHash,
                ParentFolderId = parentFolderId
            };

            var result = await _fileService.UploadFileAsync(command);

            if (!result.Success)
            {
                if (result.IsDuplicate)
                    return Conflict(ApiResponse.Fail(result.ErrorMessage!, 409));
                return BadRequest(ApiResponse.Fail(result.ErrorMessage!));
            }

            return Ok(ApiResponse.Ok(result, result.IsInstantUpload ? "秒传成功" : "上传成功"));
        }

        /// <summary>
        /// 秒传检测
        /// </summary>
        [HttpPost("upload/seconds")]
        public async Task<ActionResult<ApiResponse>> SecondsUpload([FromBody] SecondsUploadRequest request)
        {
            var userId = GetCurrentUserId();

            var result = await _fileService.TryInstantUploadAsync(
                userId, request.FileHash, request.FileName, request.MimeType, request.ParentFolderId);

            if (result.Success)
                return Ok(ApiResponse.Ok(result, "秒传成功"));

            if (result.IsDuplicate)
                return Conflict(ApiResponse.Fail(result.ErrorMessage!, 409));

            // 秒传失败，客户端需要正常上传
            return Ok(ApiResponse.Fail("文件需要正常上传"));
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            var userId = GetCurrentUserId();
            var (stream, fileName, mimeType) = await _fileService.DownloadFileAsync(id, userId);

            return File(stream, mimeType, fileName);
        }

        /// <summary>
        /// 获取文件列表（分页）
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetFileList(
            [FromQuery] Guid? parentFolderId = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true)
        {
            var userId = GetCurrentUserId();

            var query = new FileListQuery
            {
                OwnerId = userId,
                ParentFolderId = parentFolderId,
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortBy,
                Ascending = ascending
            };

            var result = await _fileService.GetFileListAsync(query);
            return Ok(ApiResponse.Ok(result));
        }

        /// <summary>
        /// 获取文件详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetFileInfo(Guid id)
        {
            var userId = GetCurrentUserId();
            var fileInfo = await _fileService.GetFileInfoAsync(id, userId);

            if (fileInfo == null)
                return NotFound(ApiResponse.Fail("文件不存在", 404));

            return Ok(ApiResponse.Ok(fileInfo));
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            await _fileService.DeleteFileAsync(new DeleteFileCommand { FileId = id, UserId = userId });
            return Ok(ApiResponse.Ok(message: "删除成功"));
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> Rename(Guid id, [FromBody] RenameRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.RenameFileAsync(new RenameFileCommand
            {
                FileId = id,
                UserId = userId,
                NewName = request.NewName
            });
            return Ok(ApiResponse.Ok(result, "重命名成功"));
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        [HttpPost("{id}/move")]
        public async Task<ActionResult<ApiResponse>> Move(Guid id, [FromBody] MoveRequest request)
        {
            var userId = GetCurrentUserId();
            await _fileService.MoveFileAsync(new MoveFileCommand
            {
                FileId = id,
                UserId = userId,
                TargetFolderId = request.TargetFolderId
            });
            return Ok(ApiResponse.Ok(message: "移动成功"));
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        [HttpPost("{id}/copy")]
        public async Task<ActionResult<ApiResponse>> Copy(Guid id, [FromBody] CopyRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.CopyFileAsync(id, userId, request.TargetFolderId);
            return Ok(ApiResponse.Ok(result, "复制成功"));
        }

        /// <summary>
        /// 搜索文件
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse>> Search(
            [FromQuery] string keyword,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var query = new FileSearchQuery
            {
                OwnerId = userId,
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var result = await _fileService.SearchFilesAsync(query);
            return Ok(ApiResponse.Ok(result));
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        [HttpPost("folder")]
        public async Task<ActionResult<ApiResponse>> CreateFolder([FromBody] CreateFolderRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.CreateFolderAsync(new CreateFolderCommand
            {
                OwnerId = userId,
                FolderName = request.FolderName,
                ParentFolderId = request.ParentFolderId
            });
            return Ok(ApiResponse.Ok(result, "文件夹创建成功"));
        }
        #region 回收站
        /// <summary>
        /// 获取回收站文件列表
        /// </summary>
        [HttpGet("trash")]
        public async Task<ActionResult<ApiResponse>> GetTrashList(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.GetTrashListAsync(userId, pageIndex, pageSize);
            return Ok(ApiResponse.Ok(result));
        }

        /// <summary>
        /// 从回收站恢复文件
        /// </summary>
        [HttpPost("{id}/restore")]
        public async Task<ActionResult<ApiResponse>> Restore(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.RestoreFileAsync(new RestoreFileCommand
            {
                FileId = id,
                UserId = userId
            });
            return Ok(ApiResponse.Ok(result, "文件恢复成功"));
        }

        /// <summary>
        /// 清空回收站
        /// </summary>
        [HttpDelete("trash")]
        public async Task<ActionResult<ApiResponse>> EmptyTrash()
        {
            var userId = GetCurrentUserId();
            var count = await _fileService.EmptyTrashAsync(userId);
            return Ok(ApiResponse.Ok(new { deletedCount = count }, $"已永久删除 {count} 个文件"));
        }
        #endregion

        #region 批量处理
        /// <summary>
        /// 批量删除文件
        /// </summary>
        [HttpPost("batch/delete")]
        public async Task<ActionResult<ApiResponse>> BatchDelete([FromBody] BatchDeleteRequest request)
        {
            var userId = GetCurrentUserId();
            var count = await _fileService.BatchDeleteAsync(new BatchDeleteCommand
            {
                FileIds = request.FileIds,
                UserId = userId
            });
            return Ok(ApiResponse.Ok(new { deletedCount = count }, $"成功删除 {count} 个文件"));
        }

        /// <summary>
        /// 批量移动文件
        /// </summary>
        [HttpPost("batch/move")]
        public async Task<ActionResult<ApiResponse>> BatchMove([FromBody] BatchMoveRequest request)
        {
            var userId = GetCurrentUserId();
            var count = await _fileService.BatchMoveAsync(new BatchMoveCommand
            {
                FileIds = request.FileIds,
                UserId = userId,
                TargetFolderId = request.TargetFolderId
            });
            return Ok(ApiResponse.Ok(new { movedCount = count }, $"成功移动 {count} 个文件"));
        }
        #endregion

        #region 分片上传

        /// <summary>
        /// 初始化分片上传会话
        /// </summary>
        [HttpPost("upload/chunk/init")]
        public async Task<ActionResult<ApiResponse>> InitChunkUpload([FromBody] InitChunkUploadRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.InitChunkUploadAsync(new InitChunkUploadCommand
            {
                OwnerId = userId,
                FileName = request.FileName,
                MimeType = request.MimeType,
                TotalSize = request.TotalSize,
                ChunkSize = request.ChunkSize,
                FileHash = request.FileHash,
                ParentFolderId = request.ParentFolderId
            });
            return Ok(ApiResponse.Ok(result, "分片上传会话已创建"));
        }

        /// <summary>
        /// 上传单个分片
        /// </summary>
        [HttpPost("upload/chunk/{sessionId}")]
        [RequestSizeLimit(100L * 1024 * 1024)] // 单片最大100MB
        public async Task<ActionResult<ApiResponse>> UploadChunk(
            Guid sessionId,
            [FromForm] int chunkIndex,
            IFormFile chunk)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.UploadChunkAsync(new UploadChunkCommand
            {
                SessionId = sessionId,
                OwnerId = userId,
                ChunkIndex = chunkIndex,
                ChunkStream = chunk.OpenReadStream()
            });
            return Ok(ApiResponse.Ok(result, $"分片 {chunkIndex} 上传成功"));
        }

        /// <summary>
        /// 完成分片上传（合并分片并创建文件）
        /// </summary>
        [HttpPost("upload/chunk/{sessionId}/complete")]
        public async Task<ActionResult<ApiResponse>> CompleteChunkUpload(Guid sessionId)
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.CompleteChunkUploadAsync(new CompleteChunkUploadCommand
            {
                SessionId = sessionId,
                OwnerId = userId
            });

            if (!result.Success)
                return BadRequest(ApiResponse.Fail(result.ErrorMessage!));

            return Ok(ApiResponse.Ok(result, "分片上传完成"));
        }

        #endregion

        #region 文件预览

        /// <summary>
        /// 文件预览（图片/PDF直接流，文本返回内容，音视频直接流）
        /// </summary>
        [HttpGet("{id}/preview")]
        public async Task<IActionResult> Preview(Guid id)
        {
            var userId = GetCurrentUserId();
            var preview = await _fileService.GetFilePreviewAsync(id, userId);

            return preview.PreviewType switch
            {
                "Stream" => File(preview.FileStream!, preview.MimeType, enableRangeProcessing: true),
                "Text" => Ok(ApiResponse.Ok(new
                {
                    preview.FileName,
                    preview.Extension,
                    preview.MimeType,
                    preview.TextContent,
                    PreviewType = "Text"
                })),
                _ => Ok(ApiResponse.Ok(new
                {
                    preview.FileName,
                    preview.Extension,
                    PreviewType = "Unsupported",
                    Message = "该文件类型不支持在线预览"
                }))
            };
        }

        #endregion

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }

    #region Request Models

    public class SecondsUploadRequest
    {
        /// <summary>
        /// 文件哈希
        /// </summary>
        public string FileHash { get; set; } = string.Empty;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// MIME类型
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// 目标文件夹ID
        /// </summary>
        public Guid? ParentFolderId { get; set; }
    }

    public class RenameRequest
    {
        /// <summary>
        /// 新文件名
        /// </summary>
        public string NewName { get; set; } = string.Empty;
    }

    public class MoveRequest
    {
        /// <summary>
        /// 目标文件夹ID
        /// </summary>
        public Guid? TargetFolderId { get; set; }
    }

    public class CopyRequest
    {
        /// <summary>
        /// 目标文件夹ID
        /// </summary>
        public Guid? TargetFolderId { get; set; }
    }

    public class CreateFolderRequest
    {
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string FolderName { get; set; } = string.Empty;

        /// <summary>
        /// 父文件夹ID
        /// </summary>
        public Guid? ParentFolderId { get; set; }
    }

    public class BatchDeleteRequest
    {
        /// <summary>
        /// 要删除的文件ID列表
        /// </summary>
        public List<Guid> FileIds { get; set; } = [];
    }

    public class BatchMoveRequest
    {
        /// <summary>
        /// 要移动的文件ID列表
        /// </summary>
        public List<Guid> FileIds { get; set; } = [];

        /// <summary>
        /// 目标文件夹ID
        /// </summary>
        public Guid? TargetFolderId { get; set; }
    }

    public class InitChunkUploadRequest
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// MIME类型
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// 文件总大小（字节）
        /// </summary>
        public long TotalSize { get; set; }

        /// <summary>
        /// 每个分片大小（字节）
        /// </summary>
        public long ChunkSize { get; set; }

        /// <summary>
        /// 文件哈希（客户端预计算，可选）
        /// </summary>
        public string? FileHash { get; set; }

        /// <summary>
        /// 目标文件夹ID
        /// </summary>
        public Guid? ParentFolderId { get; set; }
    }

    #endregion
}
