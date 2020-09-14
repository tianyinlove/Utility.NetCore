using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Utility.NetCore.Extensions
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的名称(XmlElementAttribute/XmlArrayAttribute/JsonPropertyAttribute)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this Enum value)
        {
            var result = value.ToString();
            var type = value.GetType();
            var field = type.GetField(result);
            if (field != null && field.CustomAttributes != null && field.CustomAttributes.Count() > 0)  //直接匹配一个枚举
            {
                var xeas = field.GetCustomAttributes(typeof(XmlElementAttribute), false) as XmlElementAttribute[];
                if (xeas != null && xeas.Length > 0)
                {
                    result = xeas[0].ElementName;
                }

                // 如果获取XmlElement属性为空，则去获取XmlArray属性
                if (string.IsNullOrEmpty(result))
                {
                    var xaas = field.GetCustomAttributes(typeof(XmlArrayAttribute), false) as XmlArrayAttribute[];
                    if (xaas != null && xaas.Length > 0)
                    {
                        result = xaas[0].ElementName;
                    }
                }

                // 如果获取XmlElement、XmlArray属性为空，则去获取JsonProperty属性
                if (string.IsNullOrEmpty(result))
                {
                    var xaas = field.GetCustomAttributes(typeof(JsonPropertyAttribute), false) as JsonPropertyAttribute[];
                    if (xaas != null && xaas.Length > 0)
                    {
                        result = xaas[0].PropertyName;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取枚举的说明并显示
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var field = type.GetField(value.ToString());
            if (field != null)  //直接匹配一个枚举
            {
                var att = field.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault();
                return att?.Description ?? value.ToString();
            }
            else if (type.GetCustomAttributes(false).OfType<FlagsAttribute>().Count() > 0) //多个枚举的Flags之和
            {
                if (Convert.ToInt32(value) == -1)
                {
                    return "全部";
                }
                List<string> notes = new List<string>();
                foreach (Enum item in Enum.GetValues(value.GetType()))
                {
                    if (value.HasFlag(item))
                    {
                        var att = type.GetField(item.ToString()).GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault();
                        notes.Add(att?.Description ?? item.ToString());
                    }
                }

                return string.Join("|", notes.ToArray());
            }
            return value.ToString();
        }
    }
}
