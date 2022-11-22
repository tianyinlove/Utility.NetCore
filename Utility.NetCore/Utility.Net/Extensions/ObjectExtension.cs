using System;
using System.Collections;

namespace Utility.Extensions
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ShallowClone<T>(this T obj)
                where T : class
        {
            if (obj == null)
            {
                return obj;
            }
            Type type = obj.GetType();
            T result = (T)Activator.CreateInstance(type);
            if (obj is IDictionary)
            {
                var dict = obj as IDictionary;
                foreach (var key in dict.Keys)
                {
                    (result as IDictionary)[key] = dict[key];
                }
            }
            else
            {
                foreach (var property in type.GetProperties())
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(result, property.GetValue(obj));
                    }
                }
                foreach (var field in type.GetFields())
                {
                    if (!field.IsStatic && !field.IsLiteral && !field.IsInitOnly)
                    {
                        field.SetValue(result, field.GetValue(obj));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj)
                where T : class
        {
            if (obj == null)
            {
                return obj;
            }
            Type type = obj.GetType();

            if (obj is string || type.IsValueType) //如果是字符串或值类型
            {
                return obj;
            }
            T result = (T)Activator.CreateInstance(type);
            if (obj is IDictionary)
            {
                var dict = obj as IDictionary;
                foreach (var key in dict.Keys)
                {
                    (result as IDictionary)[key] = dict[key].DeepClone();
                }
            }
            else if (obj is IList)
            {
                var list = obj as IList;
                foreach (var item in list)
                {
                    (result as IList).Add(item.DeepClone());
                }
            }
            else
            {
                foreach (var property in type.GetProperties())
                {
                    if (property.CanWrite)
                    {
                        var val = property.GetValue(obj);
                        if (val is string || property.PropertyType.IsValueType)
                        {
                            property.SetValue(result, property.GetValue(obj));
                        }
                        else
                        {
                            property.SetValue(result, property.GetValue(obj).DeepClone());
                        }
                    }
                }
                foreach (var field in type.GetFields())
                {
                    if (!field.IsStatic && !field.IsLiteral && !field.IsInitOnly)
                    {
                        field.SetValue(result, field.GetValue(obj).DeepClone());
                    }
                }
            }
            return result;
        }
    }
}
