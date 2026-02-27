using CloudDrive.Domain.ValueObjects;

namespace CloudDrive.Domain.Interfaces
{
    /// <summary>
    /// 存储提供者接口
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="fileName">文件名</param>
        /// <param name="mimeType">MIME类型</param>
        /// <returns>存储路径</returns>
        Task<FilePath> UploadAsync(Stream stream, string fileName, string mimeType);

        /// <summary>
        /// 下载文件（获取文件流）
        /// </summary>
        /// <param name="storagePath">存储路径</param>
        /// <returns>文件流</returns>
        Task<Stream> DownloadAsync(FilePath storagePath);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="storagePath">存储路径</param>
        Task DeleteAsync(FilePath storagePath);

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="storagePath">存储路径</param>
        Task<bool> ExistsAsync(FilePath storagePath);

        /// <summary>
        /// 计算文件哈希
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>文件哈希</returns>
        Task<FileHash> ComputeHashAsync(Stream stream);
    }
}
