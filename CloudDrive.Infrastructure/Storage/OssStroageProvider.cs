using Aliyun.OSS;
using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CloudDrive.Infrastructure.Storage
{
    /// <summary>
    /// 阿里云 OSS 对象存储提供者
    /// </summary>
    public class OssStorageProvider : IStorageProvider
    {
        private readonly OssOptions _ossOptions;
        private readonly ILogger<OssStorageProvider> _logger;
        private readonly OssClient _client;

        public OssStorageProvider(IOptions<StorageOptions> options, ILogger<OssStorageProvider> logger)
        {
            _ossOptions = options.Value.Oss;
            _logger = logger;
            _client = new OssClient(_ossOptions.Endpoint, _ossOptions.AccessKeyId, _ossOptions.AccessKeySecret);
        }

        public async Task<FilePath> UploadAsync(Stream stream, string fileName, string mimeType)
        {
            var datePath = DateTime.Now.ToString("yyyy/MM/dd");
            var extension = Path.GetExtension(fileName);
            var objectKey = $"{datePath}/{Guid.NewGuid():N}{extension}";

            var metadata = new ObjectMetadata
            {
                ContentType = mimeType
            };

            await Task.Run(() => _client.PutObject(_ossOptions.BucketName, objectKey, stream, metadata));

            _logger.LogInformation("文件已上传到 OSS：{ObjectKey}，Bucket：{Bucket}", objectKey, _ossOptions.BucketName);

            return new FilePath(objectKey);
        }

        public async Task<Stream> DownloadAsync(FilePath storagePath)
        {
            var result = await Task.Run(() => _client.GetObject(_ossOptions.BucketName, storagePath.path));

            // 将 OSS 响应流复制到 MemoryStream（OSS 流不可 Seek）
            var memoryStream = new MemoryStream();
            await result.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task DeleteAsync(FilePath storagePath)
        {
            await Task.Run(() => _client.DeleteObject(_ossOptions.BucketName, storagePath.path));

            _logger.LogInformation("文件已从 OSS 删除：{ObjectKey}", storagePath.path);
        }

        public async Task<bool> ExistsAsync(FilePath storagePath)
        {
            var exists = await Task.Run(() => _client.DoesObjectExist(_ossOptions.BucketName, storagePath.path));
            return exists;
        }

        public async Task<FileHash> ComputeHashAsync(Stream stream)
        {
            var originalPosition = stream.CanSeek ? stream.Position : 0;

            using var sha256 = SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            var hashString = Convert.ToHexString(hashBytes).ToLowerInvariant();

            if (stream.CanSeek)
                stream.Position = originalPosition;

            return new FileHash(hashString);
        }
    }
}
