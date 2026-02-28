namespace CloudDrive.Application.Dtos
{
    /// <summary>
    /// 分片上传会话信息DTO
    /// </summary>
    public class ChunkUploadSessionDto
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 文件总大小（字节）
        /// </summary>
        public long TotalSize { get; set; }

        /// <summary>
        /// 每个分片大小（字节）
        /// </summary>
        public long ChunkSize { get; set; }

        /// <summary>
        /// 总分片数
        /// </summary>
        public int TotalChunks { get; set; }

        /// <summary>
        /// 已上传分片数
        /// </summary>
        public int UploadedChunks { get; set; }

        /// <summary>
        /// 已上传的分片索引列表
        /// </summary>
        public List<int> UploadedChunkIndices { get; set; } = [];

        /// <summary>
        /// 会话状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}
