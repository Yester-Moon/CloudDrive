namespace CloudDrive.Domain.Enums
{
    /// <summary>
    /// 文件类型枚举
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// 文件夹
        /// </summary>
        Folder = 0,

        /// <summary>
        /// 文档（Word、Excel、PDF等）
        /// </summary>
        Document = 1,

        /// <summary>
        /// 图片
        /// </summary>
        Image = 2,

        /// <summary>
        /// 视频
        /// </summary>
        Video = 3,

        /// <summary>
        /// 音频
        /// </summary>
        Audio = 4,

        /// <summary>
        /// 压缩包
        /// </summary>
        Archive = 5,

        /// <summary>
        /// 代码文件
        /// </summary>
        Code = 6,

        /// <summary>
        /// 其他
        /// </summary>
        Other = 99
    }

    /// <summary>
    /// 文件类型辅助类
    /// </summary>
    public static class FileTypeHelper
    {
        private static readonly Dictionary<string, FileType> ExtensionMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // 文档
            { ".doc", FileType.Document },
            { ".docx", FileType.Document },
            { ".xls", FileType.Document },
            { ".xlsx", FileType.Document },
            { ".ppt", FileType.Document },
            { ".pptx", FileType.Document },
            { ".pdf", FileType.Document },
            { ".txt", FileType.Document },
            { ".md", FileType.Document },

            // 图片
            { ".jpg", FileType.Image },
            { ".jpeg", FileType.Image },
            { ".png", FileType.Image },
            { ".gif", FileType.Image },
            { ".bmp", FileType.Image },
            { ".svg", FileType.Image },
            { ".webp", FileType.Image },
            { ".ico", FileType.Image },

            // 视频
            { ".mp4", FileType.Video },
            { ".avi", FileType.Video },
            { ".mov", FileType.Video },
            { ".wmv", FileType.Video },
            { ".flv", FileType.Video },
            { ".mkv", FileType.Video },
            { ".webm", FileType.Video },

            // 音频
            { ".mp3", FileType.Audio },
            { ".wav", FileType.Audio },
            { ".flac", FileType.Audio },
            { ".aac", FileType.Audio },
            { ".ogg", FileType.Audio },
            { ".wma", FileType.Audio },

            // 压缩包
            { ".zip", FileType.Archive },
            { ".rar", FileType.Archive },
            { ".7z", FileType.Archive },
            { ".tar", FileType.Archive },
            { ".gz", FileType.Archive },

            // 代码
            { ".cs", FileType.Code },
            { ".java", FileType.Code },
            { ".js", FileType.Code },
            { ".ts", FileType.Code },
            { ".py", FileType.Code },
            { ".cpp", FileType.Code },
            { ".c", FileType.Code },
            { ".h", FileType.Code },
            { ".html", FileType.Code },
            { ".css", FileType.Code },
            { ".json", FileType.Code },
            { ".xml", FileType.Code },
        };

        /// <summary>
        /// 根据扩展名获取文件类型
        /// </summary>
        public static FileType GetFileType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return FileType.Other;

            return ExtensionMap.TryGetValue(extension, out var fileType) ? fileType : FileType.Other;
        }

        /// <summary>
        /// 获取文件类型的显示名称
        /// </summary>
        public static string GetDisplayName(FileType fileType)
        {
            return fileType switch
            {
                FileType.Folder => "文件夹",
                FileType.Document => "文档",
                FileType.Image => "图片",
                FileType.Video => "视频",
                FileType.Audio => "音频",
                FileType.Archive => "压缩包",
                FileType.Code => "代码",
                FileType.Other => "其他",
                _ => "未知"
            };
        }

        /// <summary>
        /// 获取文件类型的图标类名（用于前端显示）
        /// </summary>
        public static string GetIconClass(FileType fileType)
        {
            return fileType switch
            {
                FileType.Folder => "icon-folder",
                FileType.Document => "icon-document",
                FileType.Image => "icon-image",
                FileType.Video => "icon-video",
                FileType.Audio => "icon-audio",
                FileType.Archive => "icon-archive",
                FileType.Code => "icon-code",
                _ => "icon-file"
            };
        }
    }
}
