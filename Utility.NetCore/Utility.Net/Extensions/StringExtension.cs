using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Utility.Extensions
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 添加url参数,value会被UrlEncode，如果key已经存在就不会重复添加
        /// </summary>
        /// <param name="url">源地址</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Append(this string url, string key, string value)
        {
            //非url不添加
            if (string.IsNullOrWhiteSpace(url) || !Regex.IsMatch(url, "^(http|https)://", RegexOptions.IgnoreCase))
            {
                return url;
            }

            //如果key已经存在就不会重复添加
            if (url.ToLowerInvariant().Contains($"&{key.ToLowerInvariant()}=") || url.ToLowerInvariant().Contains($"?{key.ToLowerInvariant()}="))
            {
                return url;
            }

            if (url.Contains("?"))
            {
                return $"{url}&{key}={value}";

            }
            else
            {
                return $"{url}?{key}={value}";
            }
        }

        /// <summary>
        /// 格式化手机号码加星显示
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static string MaskPhone(this string mobile)
        {
            if (!mobile.IsPhone())
            {
                return mobile;
            }

            return string.Concat(mobile.Substring(0, 3), "****", mobile.Substring(7, 4));
        }

        /// <summary>
        /// 是否手机号码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPhone(this string value)
        {
            return Regex.IsMatch(value, @"^1[\d]{10}$");
        }

    }
}
