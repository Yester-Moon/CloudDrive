using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CloudDrive.Infrastructure.Storage
{
    /// <summary>
    /// 混合存储提供者 — 根据文件大小自动路由到热层（Local）或冷层（OSS）
    /// 小文件 → 热层（低延迟），大文件 → 冷层（低成本）
    /// </summary>
    public class HybridStorageProvider : IStorageProvider
    {
        private readonly IStorageProviderFactory _factory;
        private readonly ILogger<HybridStorageProvider> _logger;

        /// <summary>
        /// 冷层路径前缀，用于下载/删除时识别层级
        /// </summary>
        private const string ColdPrefix = "cold:";

        public HybridStorageProvider(
            IStorageProviderFactory factory,
            ILogger<HybridStorageProvider> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<FilePath> UploadAsync(Stream stream, string fileName, string mimeType)
        {
            var fileSize = stream.CanSeek ? stream.Length : 0;
            var provider = _factory.GetProviderForFileSize(fileSize);
            var isCold = ReferenceEquals(provider, _factory.GetProvider(StorageTier.Cold));

            var path = await provider.UploadAsync(stream, fileName, mimeType);

            // 给冷层路径加前缀以便后续识别
            if (isCold)
            {
                _logger.LogInformation("文件 {FileName}（{Size} 字节）存入冷层", fileName, fileSize);
                return new FilePath(ColdPrefix + path.path);
            }

            _logger.LogInformation("文件 {FileName}（{Size} 字节）存入热层", fileName, fileSize);
            return path;
        }

        public async Task<Stream> DownloadAsync(FilePath storagePath)
        {
            var (provider, realPath) = ResolveProvider(storagePath);
            return await provider.DownloadAsync(realPath);
        }

        public async Task DeleteAsync(FilePath storagePath)
        {
            var (provider, realPath) = ResolveProvider(storagePath);
            await provider.DeleteAsync(realPath);
        }

        public async Task<bool> ExistsAsync(FilePath storagePath)
        {
            var (provider, realPath) = ResolveProvider(storagePath);
            return await provider.ExistsAsync(realPath);
        }

        public async Task<FileHash> ComputeHashAsync(Stream stream)
        {
            // 哈希计算与存储层无关，使用热层提供者即可
            return await _factory.GetProvider(StorageTier.Hot).ComputeHashAsync(stream);
        }

        /// <summary>
        /// 根据路径前缀解析使用哪个存储提供者
        /// </summary>
        private (IStorageProvider Provider, FilePath RealPath) ResolveProvider(FilePath storagePath)
        {
            if (storagePath.path.StartsWith(ColdPrefix, StringComparison.Ordinal))
            {
                var realPath = new FilePath(storagePath.path[ColdPrefix.Length..]);
                return (_factory.GetProvider(StorageTier.Cold), realPath);
            }

            return (_factory.GetProvider(StorageTier.Hot), storagePath);
        }
    }
}
