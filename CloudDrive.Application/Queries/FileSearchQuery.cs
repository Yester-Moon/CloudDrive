namespace CloudDrive.Application.Queries
{
    /// <summary>
    /// 文件搜索查询
    /// </summary>
    public class FileSearchQuery
    {
        /// <summary>
        /// 所有者ID
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// 页码（从1开始）
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
