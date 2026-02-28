using CloudDrive.Common.Models;
using CloudDrive.Domain.ValueObjects;

namespace CloudDrive.Domain.Entities
{
    /// <summary>
    /// 分片上传会话实体 — 跟踪一次分片上传的整体状态
    /// </summary>
    public record ChunkUploadSession : AggregateRootEntity
    {
        /// <summary>
        /// 上传者ID
        /// </summary>
        public Guid OwnerId { get; private set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// MIME类型
        /// </summary>
        public string MimeType { get; private set; }

        /// <summary>
        /// 文件总大小（字节）
        /// </summary>
        public long TotalSize { get; private set; }

        /// <summary>
        /// 每个分片的大小（字节）
        /// </summary>
        public long ChunkSize { get; private set; }

        /// <summary>
        /// 总分片数
        /// </summary>
        public int TotalChunks { get; private set; }

        /// <summary>
        /// 已上传的分片数
        /// </summary>
        public int UploadedChunks { get; private set; }

        /// <summary>
        /// 已上传的分片索引（逗号分隔，如"0,1,3,5"）
        /// </summary>
        public string UploadedChunkIndices { get; private set; }

        /// <summary>
        /// 文件哈希（客户端预计算，用于合并后校验）
        /// </summary>
        public string? FileHash { get; private set; }

        /// <summary>
        /// 目标文件夹ID
        /// </summary>
        public Guid? ParentFolderId { get; private set; }

        /// <summary>
        /// 分片临时存储目录（相对路径）
        /// </summary>
        public string TempDirectory { get; private set; }

        /// <summary>
        /// 会话状态：Uploading / Completed / Expired / Failed
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// 会话过期时间
        /// </summary>
        public DateTime ExpiresAt { get; private set; }

        // 私有构造函数
        private ChunkUploadSession() { }

        /// <summary>
        /// 创建分片上传会话（工厂方法）
        /// </summary>
        public static ChunkUploadSession Create(
            Guid ownerId,
            string fileName,
            string mimeType,
            long totalSize,
            long chunkSize,
            string? fileHash,
            Guid? parentFolderId,
            int expirationHours = 24)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("文件名不能为空", nameof(fileName));
            if (totalSize <= 0)
                throw new ArgumentException("文件大小必须大于0", nameof(totalSize));
            if (chunkSize <= 0)
                throw new ArgumentException("分片大小必须大于0", nameof(chunkSize));

            var totalChunks = (int)Math.Ceiling((double)totalSize / chunkSize);

            var session = new ChunkUploadSession
            {
                OwnerId = ownerId,
                FileName = fileName,
                MimeType = mimeType,
                TotalSize = totalSize,
                ChunkSize = chunkSize,
                TotalChunks = totalChunks,
                UploadedChunks = 0,
                UploadedChunkIndices = string.Empty,
                FileHash = fileHash,
                ParentFolderId = parentFolderId,
                TempDirectory = $"chunks/{ownerId:N}/{Guid.NewGuid():N}",
                Status = StatusUploading,
                ExpiresAt = DateTime.Now.AddHours(expirationHours)
            };

            return session;
        }

        /// <summary>
        /// 记录某个分片已上传完成
        /// </summary>
        public void MarkChunkUploaded(int chunkIndex)
        {
            if (Status != StatusUploading)
                throw new InvalidOperationException($"会话状态为 {Status}，无法继续上传");

            if (chunkIndex < 0 || chunkIndex >= TotalChunks)
                throw new ArgumentOutOfRangeException(nameof(chunkIndex),
                    $"分片索引 {chunkIndex} 超出范围 [0, {TotalChunks - 1}]");

            var indices = GetUploadedIndicesSet();
            if (indices.Contains(chunkIndex))
                return; // 幂等：已上传则跳过

            indices.Add(chunkIndex);
            UploadedChunkIndices = string.Join(",", indices.OrderBy(i => i));
            UploadedChunks = indices.Count;
            NotifyModified();
        }

        /// <summary>
        /// 是否所有分片均已上传
        /// </summary>
        public bool IsAllChunksUploaded() => UploadedChunks >= TotalChunks;

        /// <summary>
        /// 标记会话完成
        /// </summary>
        public void MarkCompleted()
        {
            if (!IsAllChunksUploaded())
                throw new InvalidOperationException(
                    $"分片未全部上传（{UploadedChunks}/{TotalChunks}），无法完成");

            Status = StatusCompleted;
            NotifyModified();
        }

        /// <summary>
        /// 标记会话失败
        /// </summary>
        public void MarkFailed()
        {
            Status = StatusFailed;
            NotifyModified();
        }

        /// <summary>
        /// 是否已过期
        /// </summary>
        public bool IsExpired() => DateTime.Now > ExpiresAt;

        /// <summary>
        /// 检查指定分片是否已上传
        /// </summary>
        public bool IsChunkUploaded(int chunkIndex) => GetUploadedIndicesSet().Contains(chunkIndex);

        /// <summary>
        /// 获取已上传分片索引集合
        /// </summary>
        public HashSet<int> GetUploadedIndicesSet()
        {
            if (string.IsNullOrWhiteSpace(UploadedChunkIndices))
                return [];

            return UploadedChunkIndices.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToHashSet();
        }

        // 状态常量
        public const string StatusUploading = "Uploading";
        public const string StatusCompleted = "Completed";
        public const string StatusExpired = "Expired";
        public const string StatusFailed = "Failed";
    }
}
