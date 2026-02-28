using CloudDrive.Domain.Interfaces;

namespace CloudDrive.Infrastructure.Storage
{
    /// <summary>
    /// 存储提供者工厂接口 — 支持运行时按层级获取具体提供者
    /// </summary>
    public interface IStorageProviderFactory
    {
        /// <summary>
        /// 获取指定层级的存储提供者
        /// </summary>
        IStorageProvider GetProvider(StorageTier tier);

        /// <summary>
        /// 根据文件大小自动选择层级
        /// </summary>
        IStorageProvider GetProviderForFileSize(long fileSize);
    }
}
