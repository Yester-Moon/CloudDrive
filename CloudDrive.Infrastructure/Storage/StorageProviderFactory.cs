using CloudDrive.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloudDrive.Infrastructure.Storage
{
    /// <summary>
    /// 存储提供者工厂 — 按配置创建 Local / Oss 提供者
    /// </summary>
    public class StorageProviderFactory : IStorageProviderFactory
    {
        private readonly IStorageProvider _hotProvider;
        private readonly IStorageProvider _coldProvider;
        private readonly HybridStorageOptions _hybridOptions;

        public StorageProviderFactory(
            IOptions<StorageOptions> options,
            ILoggerFactory loggerFactory)
        {
            var storageOptions = options.Value;
            _hybridOptions = storageOptions.Hybrid;

            _hotProvider = CreateProvider(storageOptions.Hybrid.HotTier, options, loggerFactory);
            _coldProvider = CreateProvider(storageOptions.Hybrid.ColdTier, options, loggerFactory);
        }

        public IStorageProvider GetProvider(StorageTier tier)
        {
            return tier == StorageTier.Hot ? _hotProvider : _coldProvider;
        }

        public IStorageProvider GetProviderForFileSize(long fileSize)
        {
            return fileSize >= _hybridOptions.ColdThresholdBytes
                ? _coldProvider
                : _hotProvider;
        }

        private static IStorageProvider CreateProvider(
            string providerName,
            IOptions<StorageOptions> options,
            ILoggerFactory loggerFactory)
        {
            return providerName.ToLowerInvariant() switch
            {
                "oss" => new OssStorageProvider(options, loggerFactory.CreateLogger<OssStorageProvider>()),
                _ => new LocalStorageProvider(options, loggerFactory.CreateLogger<LocalStorageProvider>())
            };
        }
    }
}
