namespace CloudDrive.Application.Dtos
{
    /// <summary>
    /// 分页文件列表DTO
    /// </summary>
    public class FileListDto
    {
        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileInfoDto> Items { get; set; } = [];

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页码（从1开始）
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// 当前文件夹ID（null表示根目录）
        /// </summary>
        public Guid? CurrentFolderId { get; set; }

        /// <summary>
        /// 当前文件夹名称
        /// </summary>
        public string? CurrentFolderName { get; set; }
    }
}
