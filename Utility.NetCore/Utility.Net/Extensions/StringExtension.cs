using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utility.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal enum SnakeCaseState
    {
        Start,
        Lower,
        Upper,
        NewWord
    }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="flag">默认按(,|)拆分</param>
        /// <returns></returns>
        public static List<string> ToList(this string str, string[] flag)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<string>();
            }
            else
            {
                if (flag == null)
                {
                    flag = new string[] { ",", "|" };
                }
                return str.Split(flag, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        /// <summary>
        /// 将字符串转换为蛇形策略
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static string ToSnakeCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            StringBuilder sb = new StringBuilder();
            SnakeCaseState state = SnakeCaseState.Start;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (state != SnakeCaseState.Start)
                    {
                        state = SnakeCaseState.NewWord;
                    }
                }
                else if (char.IsUpper(s[i]))
                {
                    switch (state)
                    {
                        case SnakeCaseState.Upper:
                            bool hasNext = (i + 1 < s.Length);
                            if (i > 0 && hasNext)
                            {
                                char nextChar = s[i + 1];
                                if (!char.IsUpper(nextChar) && nextChar != '_')
                                {
                                    sb.Append('_');
                                }
                            }
                            break;

                        case SnakeCaseState.Lower:
                        case SnakeCaseState.NewWord:
                            sb.Append('_');
                            break;
                    }

                    sb.Append(char.ToLowerInvariant(s[i]));

                    state = SnakeCaseState.Upper;
                }
                else if (s[i] == '_')
                {
                    sb.Append('_');
                    state = SnakeCaseState.Start;
                }
                else
                {
                    if (state == SnakeCaseState.NewWord)
                    {
                        sb.Append('_');
                    }

                    sb.Append(s[i]);
                    state = SnakeCaseState.Lower;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 将字符串转换为骆驼策略
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    if (char.IsSeparator(chars[i + 1]))
                    {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
        }
    }
}
