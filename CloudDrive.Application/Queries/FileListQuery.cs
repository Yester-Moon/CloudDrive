namespace CloudDrive.Application.Queries
{
    /// <summary>
    /// 文件列表查询（分页、筛选、排序）
    /// </summary>
    public class FileListQuery
    {
        /// <summary>
        /// 所有者ID
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 文件夹ID（null表示根目录）
        /// </summary>
        public Guid? ParentFolderId { get; set; }

        /// <summary>
        /// 页码（从1开始）
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 排序字段（Name, Size, CreationTime, LastModificationTime）
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// 是否升序排序
        /// </summary>
        public bool Ascending { get; set; } = true;
    }
}
