using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Utility.Extensions
{
    /// <summary>
    /// json扩展
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// api接口json序列化格式
        /// </summary>
        public readonly static JsonSerializerSettings __apiSerializeSetting;

        /// <summary>
        /// 服务接口json序列化格式
        /// </summary>
        public readonly static JsonSerializerSettings __serviceSerializeSetting;

        static JsonExtension()
        {
            __apiSerializeSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore, //忽略空值
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                ContractResolver = new ApiCamelCasePropertyNamesContractResolver() //首字母小写
            };
            __apiSerializeSetting.Converters.Add(new DateTimeConverter());
            __apiSerializeSetting.Converters.Add(new DateTimeOffsetConverter());


            __serviceSerializeSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore, //忽略空值
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                TypeNameHandling = TypeNameHandling.Objects,
                ContractResolver = new ApiCamelCasePropertyNamesContractResolver() //首字母小写
            };
            __serviceSerializeSetting.Converters.Add(new DateTimeConverter());
            __serviceSerializeSetting.Converters.Add(new DateTimeOffsetConverter());
        }

        /// <summary>
        /// 将数据采用默认配置用Newtonsoft.Json序列化
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nullValueHandling">是否忽略空值，默认不忽略空值</param>
        /// <returns></returns>
        public static string ToJson(this object data, NullValueHandling nullValueHandling = NullValueHandling.Include)
        {
            if (data == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                NullValueHandling = nullValueHandling
            });
        }

        /// <summary>
        /// 将json采用Newtonsoft.Json默认配置反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Local });
        }

        /// <summary>
        /// 将数据json序列化，忽略空值，时间全部转换为UTC1970的毫秒数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="resolveTypeName">是否包含（继承关系）类型信息</param>
        /// <returns></returns>
        public static string ToApiJson(this object data, bool resolveTypeName = false)
        {
            if (data == null)
            {
                return null;
            }
#if NET_STD
            return JsonConvert.SerializeObject(data, Formatting.None, resolveTypeName ? __serviceSerializeSetting : __apiSerializeSetting);
#else
            //framework和standard程序集不一样，framework不用强类型解析json
            return JsonConvert.SerializeObject(data, Formatting.None, __apiSerializeSetting);
#endif
        }

        /// <summary>
        /// 将json反序列化，时间是UTC1970的毫秒数
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromApiJson(this string json, Type type)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
#if NET_STD
            try
            {
                return JsonConvert.DeserializeObject(json, type, __serviceSerializeSetting);
            }
            catch
            {
                return JsonConvert.DeserializeObject(json, type, __apiSerializeSetting);
            }
#else
            //framework和standard程序集不一样，不应该用强类型解析json
            return JsonConvert.DeserializeObject(json, type, __apiSerializeSetting);
#endif
        }

        /// <summary>
        /// 将json反序列化，时间是UTC1970的毫秒数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromApiJson<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }
#if NET_STD
            try
            {
                return JsonConvert.DeserializeObject<T>(json, __serviceSerializeSetting);
            }
            catch
            {
                return JsonConvert.DeserializeObject<T>(json, __apiSerializeSetting);
            }
#else
            //framework和standard程序集不一样，不应该用强类型解析json
            return JsonConvert.DeserializeObject<T>(json, __apiSerializeSetting);
#endif
        }

    }

    /// <summary>
    /// json序列化，时间格式转换成long(与utc1970相差的毫秒数)
    /// </summary>
    internal class DateTimeConverter : JsonConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(DateTime?) && reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.Integer)
            {
                return ((long)reader.Value).ToDateTime();
            }
            else if (reader.TokenType == JsonToken.Date)
            {
                return (DateTime)reader.Value;
            }
            else if (reader.TokenType == JsonToken.Float)
            {
                try
                {
                    return ((long)(double)reader.Value).ToDateTime();
                }
                catch
                {
                    throw new Exception($"日期格式错误,输入类型 {reader.TokenType},数据内容：{reader.Value}.");
                }
            }
            var tokenValue = reader.Value.ToString();
            if (long.TryParse((string)reader.Value, out long ticks))
            {
                return ticks.ToDateTime();
            }
            else if (DateTime.TryParse((string)reader.Value, out DateTime time))
            {
                return time;
            }
            else if (double.TryParse((string)reader.Value, out double doubleticks))
            {
                try
                {
                    return ((long)doubleticks).ToDateTime();
                }
                catch
                {
                    throw new Exception($"日期格式错误,输入类型 {reader.TokenType},数据内容：{tokenValue}.");
                }
            }
            throw new Exception($"日期格式错误,输入类型 {reader.TokenType},内容{reader.Value}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                writer.WriteValue(((DateTime)value).ValueOf());
            }
            else if (value is DateTime?)
            {
                if (value == null)
                {
                    writer.WriteValue("null");
                }
                else
                {
                    writer.WriteValue(((DateTime?)value).Value.ValueOf());
                }
            }
            else
            {
                throw new Exception("时间格式错误.2");
            }
        }
    }

    /// <summary>
    /// json序列化，时间格式转换成long(与utc1970相差的毫秒数)
    /// </summary>
    internal class DateTimeOffsetConverter : JsonConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(DateTimeOffset?) && reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.Integer)
            {
                return ((long)reader.Value).ToDateTimeOffset();
            }
            else if (reader.TokenType == JsonToken.Date)
            {
                return new DateTimeOffset((DateTime)reader.Value).ToOffset(TimeSpan.FromHours(8));
            }
            else if (reader.TokenType == JsonToken.Float)
            {
                try
                {
                    return ((long)(double)reader.Value).ToDateTimeOffset();
                }
                catch
                {
                    throw new Exception($"日期格式错误,输入类型 {reader.TokenType},数据内容：{reader.Value}.");
                }
            }
            var tokenValue = reader.Value.ToString();
            if (long.TryParse((string)reader.Value, out long ticks))
            {
                return ticks.ToDateTimeOffset();
            }
            else if (DateTimeOffset.TryParse((string)reader.Value, null, System.Globalization.DateTimeStyles.AssumeLocal, out DateTimeOffset time))
            {
                return time;
            }
            else if (double.TryParse((string)reader.Value, out double doubleticks))
            {
                try
                {
                    return ((long)doubleticks).ToDateTimeOffset();
                }
                catch
                {
                    throw new Exception($"日期格式错误,输入类型 {reader.TokenType},数据内容：{tokenValue}.");
                }
            }
            throw new Exception($"日期格式错误,输入类型 {reader.TokenType},内容{reader.Value}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTimeOffset)
            {
                writer.WriteValue(((DateTimeOffset)value).ValueOf());
            }
            else if (value is DateTimeOffset?)
            {
                if (value == null)
                {
                    writer.WriteValue("null");
                }
                else
                {
                    writer.WriteValue(((DateTimeOffset?)value).Value.ValueOf());
                }
            }
            else
            {
                throw new Exception("时间格式错误.2");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class ApiCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
    {
        static readonly DefaultContractResolver defaultContractResolver = new DefaultContractResolver();
        public override JsonContract ResolveContract(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return defaultContractResolver.ResolveContract(type);
            }
            return base.ResolveContract(type);
        }
    }
}
