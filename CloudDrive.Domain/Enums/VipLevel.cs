namespace CloudDrive.Domain.Enums
{
    /// <summary>
    /// VIP等级枚举
    /// </summary>
    public enum VipLevel
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        Normal = 0,

        /// <summary>
        /// VIP 1级 - 50GB
        /// </summary>
        Vip1 = 1,

        /// <summary>
        /// VIP 2级 - 100GB
        /// </summary>
        Vip2 = 2,

        /// <summary>
        /// VIP 3级 - 500GB
        /// </summary>
        Vip3 = 3,

        /// <summary>
        /// VIP 4级 - 1TB
        /// </summary>
        Vip4 = 4,

        /// <summary>
        /// VIP 5级 - 5TB
        /// </summary>
        Vip5 = 5
    }

    /// <summary>
    /// VIP等级辅助类
    /// </summary>
    public static class VipLevelHelper
    {
        /// <summary>
        /// 获取VIP等级的显示名称
        /// </summary>
        public static string GetDisplayName(VipLevel level)
        {
            return level switch
            {
                VipLevel.Normal => "普通用户",
                VipLevel.Vip1 => "VIP 1",
                VipLevel.Vip2 => "VIP 2",
                VipLevel.Vip3 => "VIP 3",
                VipLevel.Vip4 => "VIP 4",
                VipLevel.Vip5 => "VIP 5",
                _ => "未知"
            };
        }

        /// <summary>
        /// 获取VIP等级对应的存储配额（字节）
        /// </summary>
        public static long GetQuota(VipLevel level)
        {
            return level switch
            {
                VipLevel.Normal => 10L * 1024 * 1024 * 1024,      // 10GB
                VipLevel.Vip1 => 50L * 1024 * 1024 * 1024,        // 50GB
                VipLevel.Vip2 => 100L * 1024 * 1024 * 1024,       // 100GB
                VipLevel.Vip3 => 500L * 1024 * 1024 * 1024,       // 500GB
                VipLevel.Vip4 => 1024L * 1024 * 1024 * 1024,      // 1TB
                VipLevel.Vip5 => 5120L * 1024 * 1024 * 1024,      // 5TB
                _ => 10L * 1024 * 1024 * 1024                     // 默认10GB
            };
        }

        /// <summary>
        /// 获取格式化的配额显示
        /// </summary>
        public static string GetFormattedQuota(VipLevel level)
        {
            return level switch
            {
                VipLevel.Normal => "10 GB",
                VipLevel.Vip1 => "50 GB",
                VipLevel.Vip2 => "100 GB",
                VipLevel.Vip3 => "500 GB",
                VipLevel.Vip4 => "1 TB",
                VipLevel.Vip5 => "5 TB",
                _ => "10 GB"
            };
        }

        /// <summary>
        /// 获取VIP等级的价格（示例，单位：元/月）
        /// </summary>
        public static decimal GetMonthlyPrice(VipLevel level)
        {
            return level switch
            {
                VipLevel.Normal => 0m,
                VipLevel.Vip1 => 9.9m,
                VipLevel.Vip2 => 19.9m,
                VipLevel.Vip3 => 49.9m,
                VipLevel.Vip4 => 99.9m,
                VipLevel.Vip5 => 199.9m,
                _ => 0m
            };
        }

        /// <summary>
        /// 获取VIP等级的特权描述
        /// </summary>
        public static string[] GetPrivileges(VipLevel level)
        {
            return level switch
            {
                VipLevel.Normal => new[]
                {
                    "10GB 存储空间",
                    "单文件最大 100MB",
                    "标准下载速度"
                },
                VipLevel.Vip1 => new[]
                {
                    "50GB 存储空间",
                    "单文件最大 500MB",
                    "高速下载",
                    "无广告"
                },
                VipLevel.Vip2 => new[]
                {
                    "100GB 存储空间",
                    "单文件最大 1GB",
                    "高速下载",
                    "无广告",
                    "在线预览"
                },
                VipLevel.Vip3 => new[]
                {
                    "500GB 存储空间",
                    "单文件最大 5GB",
                    "极速下载",
                    "无广告",
                    "在线预览",
                    "文档在线编辑"
                },
                VipLevel.Vip4 => new[]
                {
                    "1TB 存储空间",
                    "单文件最大 10GB",
                    "极速下载",
                    "无广告",
                    "在线预览",
                    "文档在线编辑",
                    "版本管理"
                },
                VipLevel.Vip5 => new[]
                {
                    "5TB 存储空间",
                    "单文件不限大小",
                    "极速下载",
                    "无广告",
                    "在线预览",
                    "文档在线编辑",
                    "版本管理",
                    "团队协作"
                },
                _ => Array.Empty<string>()
            };
        }
    }
}
