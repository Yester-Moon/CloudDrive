using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CloudDrive.Infrastructure.Storage
{
    /// <summary>
    /// OSS对象存储提供者（可对接阿里云OSS、AWS S3等）
    /// 当前为接口骨架实现，集成具体SDK后替换内部逻辑
    /// </summary>
    public class OssStorageProvider : IStorageProvider
    {
        private readonly OssOptions _ossOptions;
        private readonly ILogger<OssStorageProvider> _logger;

        public OssStorageProvider(IOptions<StorageOptions> options, ILogger<OssStorageProvider> logger)
        {
            _ossOptions = options.Value.Oss;
            _logger = logger;
        }

        public async Task<FilePath> UploadAsync(Stream stream, string fileName, string mimeType)
        {
            // 按日期组织对象Key
            var datePath = DateTime.Now.ToString("yyyy/MM/dd");
            var extension = Path.GetExtension(fileName);
            var objectKey = $"{datePath}/{Guid.NewGuid():N}{extension}";

            // TODO: 集成OSS SDK
            // var client = new OssClient(_ossOptions.Endpoint, _ossOptions.AccessKeyId, _ossOptions.AccessKeySecret);
            // client.PutObject(_ossOptions.BucketName, objectKey, stream);

            _logger.LogWarning("OSS上传尚未集成SDK，对象Key：{ObjectKey}。请安装OSS SDK并实现上传逻辑。", objectKey);

            await Task.CompletedTask;
            return new FilePath(objectKey);
        }

        public async Task<Stream> DownloadAsync(FilePath storagePath)
        {
            // TODO: 集成OSS SDK
            // var client = new OssClient(_ossOptions.Endpoint, _ossOptions.AccessKeyId, _ossOptions.AccessKeySecret);
            // var result = client.GetObject(_ossOptions.BucketName, storagePath.path);
            // return result.Content;

            _logger.LogWarning("OSS下载尚未集成SDK，对象Key：{ObjectKey}", storagePath.path);

            await Task.CompletedTask;
            throw new NotImplementedException("OSS存储提供者尚未集成SDK，请安装Aliyun.OSS.SDK.NetCore或对应SDK。");
        }

        public async Task DeleteAsync(FilePath storagePath)
        {
            // TODO: 集成OSS SDK
            // var client = new OssClient(_ossOptions.Endpoint, _ossOptions.AccessKeyId, _ossOptions.AccessKeySecret);
            // client.DeleteObject(_ossOptions.BucketName, storagePath.path);

            _logger.LogWarning("OSS删除尚未集成SDK，对象Key：{ObjectKey}", storagePath.path);

            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(FilePath storagePath)
        {
            // TODO: 集成OSS SDK
            // var client = new OssClient(_ossOptions.Endpoint, _ossOptions.AccessKeyId, _ossOptions.AccessKeySecret);
            // return client.DoesObjectExist(_ossOptions.BucketName, storagePath.path);

            _logger.LogWarning("OSS存在性检查尚未集成SDK，对象Key：{ObjectKey}", storagePath.path);

            await Task.CompletedTask;
            return false;
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
