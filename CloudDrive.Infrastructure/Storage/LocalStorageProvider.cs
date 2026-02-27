using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CloudDrive.Infrastructure.Storage
{
    /// <summary>
    /// 本地文件存储提供者
    /// </summary>
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly StorageOptions _options;
        private readonly ILogger<LocalStorageProvider> _logger;

        public LocalStorageProvider(IOptions<StorageOptions> options, ILogger<LocalStorageProvider> logger)
        {
            _options = options.Value;
            _logger = logger;

            // 确保根存储目录存在
            if (!Directory.Exists(_options.LocalStoragePath))
            {
                Directory.CreateDirectory(_options.LocalStoragePath);
            }
        }

        public async Task<FilePath> UploadAsync(Stream stream, string fileName, string mimeType)
        {
            // 按日期组织目录：yyyy/MM/dd/
            var datePath = DateTime.Now.ToString("yyyy/MM/dd");
            var directoryPath = Path.Combine(_options.LocalStoragePath, datePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 生成唯一文件名，保留扩展名
            var extension = Path.GetExtension(fileName);
            var uniqueName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(directoryPath, uniqueName);

            // 存储相对路径
            var relativePath = Path.Combine(datePath, uniqueName).Replace('\\', '/');

            await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);

            _logger.LogInformation("文件已上传到本地存储：{Path}", relativePath);

            return new FilePath(relativePath);
        }

        public async Task<Stream> DownloadAsync(FilePath storagePath)
        {
            var fullPath = Path.Combine(_options.LocalStoragePath, storagePath.path);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"文件不存在：{storagePath.path}");

            // 返回文件流（调用方负责释放）
            var memoryStream = new MemoryStream();
            await using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }

        public Task DeleteAsync(FilePath storagePath)
        {
            var fullPath = Path.Combine(_options.LocalStoragePath, storagePath.path);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("文件已从本地存储删除：{Path}", storagePath.path);
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(FilePath storagePath)
        {
            var fullPath = Path.Combine(_options.LocalStoragePath, storagePath.path);
            return Task.FromResult(File.Exists(fullPath));
        }

        public async Task<FileHash> ComputeHashAsync(Stream stream)
        {
            var originalPosition = stream.CanSeek ? stream.Position : 0;

            using var sha256 = SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            var hashString = Convert.ToHexString(hashBytes).ToLowerInvariant();

            // 重置流位置
            if (stream.CanSeek)
                stream.Position = originalPosition;

            return new FileHash(hashString);
        }
    }
}
