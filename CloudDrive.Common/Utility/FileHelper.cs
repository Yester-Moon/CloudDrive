namespace CloudDrive.Common.Utility
{
    /// <summary>
    /// 文件工具类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 常见 MIME 类型映射表
        /// </summary>
        private static readonly Dictionary<string, string> MimeTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // 文档
            { ".txt", "text/plain" },
            { ".pdf", "application/pdf" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".ppt", "application/vnd.ms-powerpoint" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ".csv", "text/csv" },
            { ".rtf", "application/rtf" },
            { ".md", "text/markdown" },

            // 图片
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".bmp", "image/bmp" },
            { ".svg", "image/svg+xml" },
            { ".webp", "image/webp" },
            { ".ico", "image/x-icon" },
            { ".tiff", "image/tiff" },
            { ".tif", "image/tiff" },

            // 视频
            { ".mp4", "video/mp4" },
            { ".avi", "video/x-msvideo" },
            { ".mov", "video/quicktime" },
            { ".wmv", "video/x-ms-wmv" },
            { ".flv", "video/x-flv" },
            { ".mkv", "video/x-matroska" },
            { ".webm", "video/webm" },

            // 音频
            { ".mp3", "audio/mpeg" },
            { ".wav", "audio/wav" },
            { ".flac", "audio/flac" },
            { ".aac", "audio/aac" },
            { ".ogg", "audio/ogg" },
            { ".wma", "audio/x-ms-wma" },

            // 压缩包
            { ".zip", "application/zip" },
            { ".rar", "application/vnd.rar" },
            { ".7z", "application/x-7z-compressed" },
            { ".tar", "application/x-tar" },
            { ".gz", "application/gzip" },

            // 代码 / 配置
            { ".html", "text/html" },
            { ".htm", "text/html" },
            { ".css", "text/css" },
            { ".js", "application/javascript" },
            { ".json", "application/json" },
            { ".xml", "application/xml" },
            { ".yaml", "application/x-yaml" },
            { ".yml", "application/x-yaml" },

            // 其它
            { ".apk", "application/vnd.android.package-archive" },
            { ".exe", "application/x-msdownload" },
            { ".iso", "application/x-iso9660-image" },
        };

        /// <summary>
        /// 根据文件扩展名获取 MIME 类型
        /// </summary>
        /// <param name="fileName">文件名或扩展名（如 "test.pdf" 或 ".pdf"）</param>
        /// <returns>对应的 MIME 类型，未知类型返回 application/octet-stream</returns>
        public static string GetMimeType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "application/octet-stream";

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                return "application/octet-stream";

            return MimeTypeMap.TryGetValue(extension, out var mimeType)
                ? mimeType
                : "application/octet-stream";
        }

        /// <summary>
        /// 根据 MIME 类型判断是否为图片
        /// </summary>
        public static bool IsImage(string mimeType)
        {
            return !string.IsNullOrEmpty(mimeType) && mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 根据 MIME 类型判断是否为视频
        /// </summary>
        public static bool IsVideo(string mimeType)
        {
            return !string.IsNullOrEmpty(mimeType) && mimeType.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 根据 MIME 类型判断是否为音频
        /// </summary>
        public static bool IsAudio(string mimeType)
        {
            return !string.IsNullOrEmpty(mimeType) && mimeType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 根据 MIME 类型判断是否为文本
        /// </summary>
        public static bool IsText(string mimeType)
        {
            return !string.IsNullOrEmpty(mimeType) && mimeType.StartsWith("text/", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 格式化文件大小为可读字符串
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化后的字符串，如 "1.50 MB"</returns>
        public static string FormatFileSize(long bytes)
        {
            if (bytes < 0)
                return "0 B";

            string[] units = ["B", "KB", "MB", "GB", "TB", "PB"];
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < units.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return order == 0
                ? $"{size:0} {units[order]}"
                : $"{size:0.##} {units[order]}";
        }

        /// <summary>
        /// 获取安全的文件名（移除非法字符）
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <returns>安全的文件名</returns>
        public static string GetSafeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "unnamed";

            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());

            return string.IsNullOrWhiteSpace(safeName) ? "unnamed" : safeName;
        }

        /// <summary>
        /// 生成唯一文件名（保留原始扩展名）
        /// </summary>
        /// <param name="originalFileName">原始文件名</param>
        /// <returns>唯一文件名</returns>
        public static string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            return $"{Guid.NewGuid():N}{extension}";
        }

        /// <summary>
        /// 获取文件扩展名（统一小写，包含"."）
        /// </summary>
        public static string GetExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            return Path.GetExtension(fileName).ToLowerInvariant();
        }
    }
}
