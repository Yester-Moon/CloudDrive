namespace CloudDrive.Common.Utility
{
    /// <summary>
    /// 日期时间工具类
    /// </summary>
    public static class DateTimeHelper
    {
        private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 获取友好的相对时间描述
        /// </summary>
        /// <param name="dateTime">目标时间</param>
        /// <returns>友好时间字符串，如 "3分钟前"、"2天前"</returns>
        public static string ToFriendlyString(DateTime dateTime)
        {
            var now = DateTime.Now;
            var span = now - dateTime;

            if (span.TotalSeconds < 0)
                return "刚刚";

            if (span.TotalSeconds < 60)
                return "刚刚";

            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes}分钟前";

            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours}小时前";

            if (span.TotalDays < 7)
                return $"{(int)span.TotalDays}天前";

            if (span.TotalDays < 30)
                return $"{(int)(span.TotalDays / 7)}周前";

            if (span.TotalDays < 365)
                return $"{(int)(span.TotalDays / 30)}个月前";

            return $"{(int)(span.TotalDays / 365)}年前";
        }

        /// <summary>
        /// DateTime 转 Unix 时间戳（秒）
        /// </summary>
        /// <param name="dateTime">UTC 时间</param>
        /// <returns>Unix 时间戳（秒）</returns>
        public static long ToUnixTimestamp(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Unix 时间戳（秒）转 DateTime（UTC）
        /// </summary>
        /// <param name="timestamp">Unix 时间戳（秒）</param>
        /// <returns>UTC DateTime</returns>
        public static DateTime FromUnixTimestamp(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        }

        /// <summary>
        /// DateTime 转 Unix 时间戳（毫秒）
        /// </summary>
        /// <param name="dateTime">UTC 时间</param>
        /// <returns>Unix 时间戳（毫秒）</returns>
        public static long ToUnixTimestampMs(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Unix 时间戳（毫秒）转 DateTime（UTC）
        /// </summary>
        /// <param name="timestampMs">Unix 时间戳（毫秒）</param>
        /// <returns>UTC DateTime</returns>
        public static DateTime FromUnixTimestampMs(long timestampMs)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).UtcDateTime;
        }

        /// <summary>
        /// 获取指定日期当天的开始时间（00:00:00）
        /// </summary>
        public static DateTime StartOfDay(DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// 获取指定日期当天的结束时间（23:59:59.9999999）
        /// </summary>
        public static DateTime EndOfDay(DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// 判断时间是否已过期
        /// </summary>
        /// <param name="expirationTime">过期时间</param>
        /// <returns>是否已过期</returns>
        public static bool IsExpired(DateTime? expirationTime)
        {
            return expirationTime.HasValue && expirationTime.Value < DateTime.Now;
        }

        /// <summary>
        /// 获取剩余时间的友好描述
        /// </summary>
        /// <param name="expirationTime">过期时间</param>
        /// <returns>剩余时间描述，如 "剩余2天3小时"</returns>
        public static string GetRemainingTimeString(DateTime? expirationTime)
        {
            if (!expirationTime.HasValue)
                return "永不过期";

            var remaining = expirationTime.Value - DateTime.Now;
            if (remaining.TotalSeconds <= 0)
                return "已过期";

            if (remaining.TotalMinutes < 60)
                return $"剩余{(int)remaining.TotalMinutes}分钟";

            if (remaining.TotalHours < 24)
                return $"剩余{(int)remaining.TotalHours}小时{remaining.Minutes}分钟";

            if (remaining.TotalDays < 30)
                return $"剩余{(int)remaining.TotalDays}天{remaining.Hours}小时";

            return $"剩余{(int)(remaining.TotalDays / 30)}个月";
        }

        /// <summary>
        /// 格式化为标准日期时间字符串
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <param name="format">格式字符串，默认 "yyyy-MM-dd HH:mm:ss"</param>
        /// <returns>格式化后的字符串</returns>
        public static string Format(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }
    }
}
