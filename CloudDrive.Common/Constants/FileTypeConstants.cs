namespace CloudDrive.Common.Constants
{
    /// <summary>
    /// 文件类型相关常量
    /// </summary>
    public static class FileTypeConstants
    {
        /// <summary>
        /// 禁止上传的文件扩展名（黑名单）
        /// </summary>
        public static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".exe", ".bat", ".cmd", ".com", ".scr",
            ".pif", ".vbs", ".js", ".wsf", ".msi",
            ".dll", ".sys", ".reg"
        };

        /// <summary>
        /// 可在线预览的图片扩展名
        /// </summary>
        public static readonly HashSet<string> PreviewableImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".ico"
        };

        /// <summary>
        /// 可在线预览的文本扩展名
        /// </summary>
        public static readonly HashSet<string> PreviewableTextExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".md", ".csv", ".log", ".json", ".xml", ".yaml", ".yml",
            ".html", ".htm", ".css", ".cs", ".java", ".py", ".js", ".ts",
            ".c", ".cpp", ".h", ".go", ".rs", ".sh", ".bat", ".sql"
        };

        /// <summary>
        /// 可在线预览的视频扩展名
        /// </summary>
        public static readonly HashSet<string> PreviewableVideoExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp4", ".webm"
        };

        /// <summary>
        /// 可在线预览的音频扩展名
        /// </summary>
        public static readonly HashSet<string> PreviewableAudioExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3", ".wav", ".ogg", ".aac"
        };

        /// <summary>
        /// 判断扩展名是否在黑名单中
        /// </summary>
        public static bool IsBlocked(string extension)
        {
            return !string.IsNullOrEmpty(extension) && BlockedExtensions.Contains(extension);
        }

        /// <summary>
        /// 判断文件是否支持在线预览
        /// </summary>
        public static bool IsPreviewable(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;

            return PreviewableImageExtensions.Contains(extension)
                || PreviewableTextExtensions.Contains(extension)
                || PreviewableVideoExtensions.Contains(extension)
                || PreviewableAudioExtensions.Contains(extension);
        }
    }
}
