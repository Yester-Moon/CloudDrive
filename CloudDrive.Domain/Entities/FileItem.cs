using CloudDrive.Common.Models;
using CloudDrive.Domain.DomainEvents;
using CloudDrive.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDrive.Domain.Entities
{
    /// <summary>
    /// 文件项实体 - 聚合根
    /// </summary>
    public record FileItem : AggregateRootEntity
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 文件扩展名（含点，如：.pdf）
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// 文件大小（值对象）
        /// </summary>
        public FileSize Size { get; private set; }

        /// <summary>
        /// 文件物理存储路径（值对象）
        /// </summary>
        public FilePath StoragePath { get; private set; }

        /// <summary>
        /// 文件哈希值（值对象，用于去重）
        /// </summary>
        public FileHash Hash { get; private set; }

        /// <summary>
        /// MIME类型（如：application/pdf）
        /// </summary>
        public string MimeType { get; private set; }

        /// <summary>
        /// 文件所有者ID
        /// </summary>
        public Guid OwnerId { get; private set; }

        /// <summary>
        /// 父文件夹ID（根目录为null）
        /// </summary>
        public Guid? ParentFolderId { get; private set; }

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool IsFolder { get; private set; }

        /// <summary>
        /// 文件下载次数
        /// </summary>
        public int DownloadCount { get; private set; }

        /// <summary>
        /// 文件标签（逗号分隔）
        /// </summary>
        public string? Tags { get; private set; }

        /// <summary>
        /// 文件备注/描述
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// 缩略图URL（图片、视频等）
        /// </summary>
        public string? ThumbnailUrl { get; private set; }

        /// <summary>
        /// 所有者导航属性
        /// </summary>
        public User? Owner { get; private set; }

        // 私有构造函数，强制使用工厂方法
        private FileItem() { }

        /// <summary>
        /// 创建文件项（工厂方法）
        /// </summary>
        public static FileItem CreateFile(
            string name,
            FileSize size,
            FilePath storagePath,
            FileHash hash,
            string mimeType,
            Guid ownerId,
            Guid? parentFolderId = null)
        {
            // 业务规则验证
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("文件名不能为空", nameof(name));

            if (size.bytesize <= 0)
                throw new ArgumentException("文件大小必须大于0", nameof(size));

            var extension = Path.GetExtension(name);

            var fileItem = new FileItem
            {
                Name = name,
                Extension = extension,
                Size = size,
                StoragePath = storagePath,
                Hash = hash,
                MimeType = mimeType,
                OwnerId = ownerId,
                ParentFolderId = parentFolderId,
                IsFolder = false,
                DownloadCount = 0
            };

            // 触发领域事件
            fileItem.AddNotification(new FileUploadedEvent(fileItem.Id, ownerId, size.bytesize));

            return fileItem;
        }

        /// <summary>
        /// 创建文件夹（工厂方法）
        /// </summary>
        public static FileItem CreateFolder(
            string name,
            Guid ownerId,
            Guid? parentFolderId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("文件夹名不能为空", nameof(name));

            var folder = new FileItem
            {
                Name = name,
                Extension = string.Empty,
                Size = new FileSize(0),
                StoragePath = new FilePath(string.Empty),
                Hash = new FileHash(string.Empty),
                MimeType = "application/folder",
                OwnerId = ownerId,
                ParentFolderId = parentFolderId,
                IsFolder = true,
                DownloadCount = 0
            };

            return folder;
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("新文件名不能为空", nameof(newName));

            Name = newName;
            if (!IsFolder)
            {
                Extension = Path.GetExtension(newName);
            }
            NotifyModified();
        }

        /// <summary>
        /// 移动文件到新文件夹
        /// </summary>
        public void MoveTo(Guid? newParentFolderId)
        {
            ParentFolderId = newParentFolderId;
            NotifyModified();
        }

        /// <summary>
        /// 增加下载次数
        /// </summary>
        public void IncrementDownloadCount()
        {
            DownloadCount++;
            NotifyModified();
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        public void UpdateTags(string tags)
        {
            Tags = tags;
            NotifyModified();
        }

        /// <summary>
        /// 更新描述
        /// </summary>
        public void UpdateDescription(string description)
        {
            Description = description;
            NotifyModified();
        }

        /// <summary>
        /// 设置缩略图
        /// </summary>
        public void SetThumbnail(string thumbnailUrl)
        {
            ThumbnailUrl = thumbnailUrl;
            NotifyModified();
        }

        /// <summary>
        /// 删除文件（软删除）
        /// </summary>
        public override void SoftDelete()
        {
            base.SoftDelete();
            // 触发文件删除事件
            AddNotification(new FileDeletedEvent(Id, OwnerId, Size.bytesize));
        }

        /// <summary>
        /// 验证文件类型是否允许
        /// </summary>
        public bool IsAllowedFileType(string[] allowedExtensions)
        {
            if (IsFolder) return true;
            return allowedExtensions.Contains(Extension.ToLowerInvariant());
        }

        /// <summary>
        /// 获取格式化的文件大小
        /// </summary>
        public string GetFormattedSize()
        {
            if (IsFolder) return "-";

            var size = Size.bytesize;
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = size;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
