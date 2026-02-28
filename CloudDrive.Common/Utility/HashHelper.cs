using System.Security.Cryptography;
using System.Text;

namespace CloudDrive.Common.Utility
{
    /// <summary>
    /// 哈希计算工具类
    /// </summary>
    public static class HashHelper
    {
        private const int DefaultBufferSize = 81920; // 80 KB

        #region MD5

        /// <summary>
        /// 计算字符串的 MD5 哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>小写十六进制哈希值</returns>
        public static string ComputeMd5(string input)
        {
            ArgumentNullException.ThrowIfNull(input);
            var bytes = Encoding.UTF8.GetBytes(input);
            return ComputeMd5(bytes);
        }

        /// <summary>
        /// 计算字节数组的 MD5 哈希值
        /// </summary>
        /// <param name="data">输入字节数组</param>
        /// <returns>小写十六进制哈希值</returns>
        public static string ComputeMd5(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);
            var hash = MD5.HashData(data);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        /// <summary>
        /// 计算流的 MD5 哈希值
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>小写十六进制哈希值</returns>
        public static string ComputeMd5(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(stream);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        /// <summary>
        /// 异步计算流的 MD5 哈希值
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>小写十六进制哈希值</returns>
        public static async Task<string> ComputeMd5Async(Stream stream, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stream);
            var hash = await MD5.HashDataAsync(stream, cancellationToken);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        /// <summary>
        /// 计算文件的 MD5 哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>小写十六进制哈希值</returns>
        public static async Task<string> ComputeFileMd5Async(string filePath, CancellationToken cancellationToken = default)
        {
            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, useAsync: true);
            return await ComputeMd5Async(stream, cancellationToken);
        }

        #endregion

        #region SHA256

        /// <summary>
        /// 计算字符串的 SHA256 哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>小写十六进制哈希值</returns>
        public static string ComputeSha256(string input)
        {
            ArgumentNullException.ThrowIfNull(input);
            var bytes = Encoding.UTF8.GetBytes(input);
            return ComputeSha256(bytes);
        }

        /// <summary>
        /// 计算字节数组的 SHA256 哈希值
        /// </summary>
        /// <param name="data">输入字节数组</param>
        /// <returns>小写十六进制哈希值</returns>
        public static string ComputeSha256(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);
            var hash = SHA256.HashData(data);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        /// <summary>
        /// 计算流的 SHA256 哈希值
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>小写十六进制哈希值</returns>
        public static string ComputeSha256(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(stream);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        /// <summary>
        /// 异步计算流的 SHA256 哈希值
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>小写十六进制哈希值</returns>
        public static async Task<string> ComputeSha256Async(Stream stream, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stream);
            var hash = await SHA256.HashDataAsync(stream, cancellationToken);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        /// <summary>
        /// 计算文件的 SHA256 哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>小写十六进制哈希值</returns>
        public static async Task<string> ComputeFileSha256Async(string filePath, CancellationToken cancellationToken = default)
        {
            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, useAsync: true);
            return await ComputeSha256Async(stream, cancellationToken);
        }

        #endregion

        /// <summary>
        /// 使用 HMAC-SHA256 计算签名
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="data">要签名的数据</param>
        /// <returns>小写十六进制签名</returns>
        public static string HmacSha256(string key, string data)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(data);

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var hash = HMACSHA256.HashData(keyBytes, dataBytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
