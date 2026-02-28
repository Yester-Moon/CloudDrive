using System.Text;
using System.Text.RegularExpressions;

namespace CloudDrive.Common.Utility
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    public static partial class StringHelper
    {
        private static readonly char[] AlphaNumericChars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        /// <summary>
        /// 截断字符串到指定长度，超出部分用省略号替换
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="maxLength">最大长度（含省略号）</param>
        /// <param name="suffix">省略号字符，默认 "..."</param>
        /// <returns>截断后的字符串</returns>
        public static string Truncate(string? value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value ?? string.Empty;

            if (maxLength <= suffix.Length)
                return suffix[..maxLength];

            return string.Concat(value.AsSpan(0, maxLength - suffix.Length), suffix);
        }

        /// <summary>
        /// 遮罩字符串（如手机号、邮箱脱敏）
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="prefixLength">前缀保留长度</param>
        /// <param name="suffixLength">后缀保留长度</param>
        /// <param name="maskChar">遮罩字符</param>
        /// <returns>遮罩后的字符串</returns>
        public static string Mask(string? value, int prefixLength = 3, int suffixLength = 4, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Length <= prefixLength + suffixLength)
                return new string(maskChar, value.Length);

            var maskLength = value.Length - prefixLength - suffixLength;
            return string.Concat(
                value.AsSpan(0, prefixLength),
                new string(maskChar, maskLength),
                value.AsSpan(value.Length - suffixLength));
        }

        /// <summary>
        /// 将字符串转换为 URL 友好的 slug
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <returns>slug 字符串</returns>
        public static string ToSlug(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // 转小写，替换空格和特殊字符为连字符
            var slug = value.Trim().ToLowerInvariant();
            slug = SlugInvalidCharsRegex().Replace(slug, "-");
            slug = SlugConsecutiveHyphensRegex().Replace(slug, "-");
            slug = slug.Trim('-');

            return slug;
        }

        /// <summary>
        /// 生成指定长度的随机字母数字字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns>随机字符串</returns>
        public static string GenerateRandom(int length)
        {
            if (length <= 0)
                return string.Empty;

            return string.Create(length, 0, static (span, _) =>
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".AsSpan();
                for (var i = 0; i < span.Length; i++)
                    span[i] = chars[Random.Shared.Next(chars.Length)];
            });
        }

        /// <summary>
        /// 生成指定长度的随机数字字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns>随机数字字符串</returns>
        public static string GenerateRandomDigits(int length)
        {
            if (length <= 0)
                return string.Empty;

            return string.Create(length, 0, static (span, _) =>
            {
                for (var i = 0; i < span.Length; i++)
                    span[i] = (char)('0' + Random.Shared.Next(10));
            });
        }

        /// <summary>
        /// 邮箱脱敏（保留首字符和@后域名）
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <returns>脱敏后的邮箱</returns>
        public static string MaskEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            var atIndex = email.IndexOf('@');
            if (atIndex <= 0)
                return Mask(email, 1, 0);

            var localPart = email[..atIndex];
            var domainPart = email[atIndex..];

            if (localPart.Length <= 2)
                return string.Concat(localPart.AsSpan(0, 1), "***", domainPart);

            return string.Concat(localPart.AsSpan(0, 2), "***", domainPart);
        }

        /// <summary>
        /// 判断字符串是否为有效的邮箱格式
        /// </summary>
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex().IsMatch(email);
        }

        /// <summary>
        /// 将字节数组转换为十六进制字符串
        /// </summary>
        public static string ToHexString(byte[] bytes)
        {
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        [GeneratedRegex(@"[^a-z0-9\s\-]")]
        private static partial Regex SlugInvalidCharsRegex();

        [GeneratedRegex(@"-{2,}")]
        private static partial Regex SlugConsecutiveHyphensRegex();

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}
