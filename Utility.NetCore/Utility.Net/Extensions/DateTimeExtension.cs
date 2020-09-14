using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Extensions
{
    /// <summary>
    /// 时间扩展
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Utc1970时间的localtime
        /// </summary>
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTimeOffset EpochOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private static readonly long MinValue = (long)(new DateTime() - Epoch).TotalMilliseconds;

        /// <summary>
        /// 从 UTC 1970 年 1 月 1 日开始计的毫秒数 。
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static long ValueOf(this DateTime datetime)
        {
            long value;
            if (datetime.Kind == DateTimeKind.Unspecified)      //未明确时区的都认为是+8区
            {
                value = (long)(datetime - Epoch).Add(TimeSpan.FromHours(-8)).TotalMilliseconds;
            }
            else
            {
                value = (long)(datetime.ToUniversalTime() - Epoch).TotalMilliseconds;
            }
            return value <= MinValue ? MinValue : value;
        }

        /// <summary>
        /// 从 UTC 1970 年 1 月 1 日开始计的毫秒数 。
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static long ValueOf(this DateTimeOffset datetime)
        {
            return (long)(datetime - EpochOffset).TotalMilliseconds;
        }

        /// <summary>
        /// 返回当前时间
        /// </summary>
        /// <param name="dateVal"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long dateVal)
        {
            return Epoch.AddMilliseconds(dateVal).ToLocalTime();
        }

        /// <summary>
        /// 返回当前时间
        /// </summary>
        /// <param name="dateVal"></param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTimeOffset(this long dateVal)
        {
            return EpochOffset.AddMilliseconds(dateVal).ToOffset(TimeSpan.FromHours(8));
        }

    }
}
