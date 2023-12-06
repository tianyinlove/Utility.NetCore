using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

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

        /// <summary>
        /// 深拷贝(利用表达式树实现深拷贝)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T obj)
               where T : class
        {
            return Copier<T, T>.Copy(obj);
        }

        /// <summary>
        /// 深拷贝(利用表达式树实现深拷贝)源对象的属性值拷贝至目标对象的对应属性
        /// </summary>
        /// <typeparam name="TSource">源对象</typeparam>
        /// <typeparam name="TTarget">目标对象</typeparam>
        /// <param name="source">源数据</param>
        /// <returns></returns>
        public static TTarget DeepCopy<TSource, TTarget>(this TSource source)
               where TSource : class
        {
            return Copier<TSource, TTarget>.Copy(source);
        }
    }

    /// <summary>
    /// 利用表达式树实现深拷贝的类
    /// </summary>
    /// <typeparam name="TSource">源对象类型</typeparam>
    /// <typeparam name="TTarget">目标对象类型</typeparam>
    internal static class Copier<TSource, TTarget>
    {
        // 缓存委托
        private static Func<TSource, TTarget> _copyFunc;
        private static Action<TSource, TTarget> _copyAction;

        /// <summary>
        /// 新建目标类型实例，并将源对象的属性值拷贝至目标对象的对应属性
        /// </summary>
        /// <param name="source">源对象实例</param>
        /// <returns>深拷贝了源对象属性的目标对象实例</returns>
        public static TTarget Copy(TSource source)
        {
            if (source == null) return default(TTarget);

            // 因为对于泛型类型而言，每次传入不同的泛型参数都会调用静态构造函数，所以可以通过这种方式进行缓存
            if (_copyFunc != null)
            {
                // 如果之前缓存过，则直接调用缓存的委托
                return _copyFunc(source);
            }

            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            var paramExpr = Expression.Parameter(sourceType, nameof(source));

            Expression bodyExpr;

            // 如果对象可以遍历（目前只支持数组和ICollection<T>实现类）
            if (sourceType == targetType && Utils.IsIEnumerableExceptString(sourceType))
            {
                bodyExpr = Expression.Call(null, EnumerableCopier.GetMethondInfo(sourceType), paramExpr);
            }
            else
            {
                var memberBindings = new List<MemberBinding>();
                // 遍历目标对象的所有属性信息
                foreach (var targetPropInfo in targetType.GetProperties())
                {
                    // 从源对象获取同名的属性信息
                    var sourcePropInfo = sourceType.GetProperty(targetPropInfo.Name);

                    Type sourcePropType = sourcePropInfo?.PropertyType;
                    Type targetPropType = targetPropInfo.PropertyType;

                    // 只在满足以下三个条件的情况下进行拷贝
                    // 1.源属性类型和目标属性类型一致
                    // 2.源属性可读
                    // 3.目标属性可写
                    if (sourcePropType == targetPropType
                        && sourcePropInfo.CanRead
                        && targetPropInfo.CanWrite)
                    {
                        // 获取属性值的表达式
                        Expression expression = Expression.Property(paramExpr, sourcePropInfo);

                        // 如果目标属性是值类型或者字符串，则直接做赋值处理
                        // 暂不考虑目标值类型有非字符串的引用类型这种特殊情况
                        // 非字符串引用类型做递归处理
                        if (Utils.IsRefTypeExceptString(targetPropType))
                        {
                            // 进行递归
                            if (Utils.IsRefTypeExceptString(targetPropType))
                            {
                                expression = Expression.Call(null,
                                    GetCopyMethodInfo(sourcePropType, targetPropType), expression);
                            }
                        }
                        memberBindings.Add(Expression.Bind(targetPropInfo, expression));
                    }
                }

                bodyExpr = Expression.MemberInit(Expression.New(targetType), memberBindings);
            }

            var lambdaExpr
                = Expression.Lambda<Func<TSource, TTarget>>(bodyExpr, paramExpr);

            _copyFunc = lambdaExpr.Compile();
            return _copyFunc(source);
        }

        /// <summary>
        /// 新建目标类型实例，并将源对象的属性值拷贝至目标对象的对应属性
        /// </summary>
        /// <param name="source">源对象实例</param>
        /// <param name="target">目标对象实例</param>
        public static void Copy(TSource source, TTarget target)
        {
            if (source == null) return;

            // 因为对于泛型类型而言，每次传入不同的泛型参数都会调用静态构造函数，所以可以通过这种方式进行缓存
            // 如果之前缓存过，则直接调用缓存的委托
            if (_copyAction != null)
            {
                _copyAction(source, target);
                return;
            }

            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            // 如果双方都可以被遍历
            if (Utils.IsIEnumerableExceptString(sourceType) && Utils.IsIEnumerableExceptString(targetType))
            {
                // TODO
                // 向已存在的数组或者ICollection<T>拷贝的功能暂不支持
            }
            else
            {
                var paramSourceExpr = Expression.Parameter(sourceType, nameof(source));
                var paramTargetExpr = Expression.Parameter(targetType, nameof(target));

                var binaryExpressions = new List<Expression>();
                // 遍历目标对象的所有属性信息
                foreach (var targetPropInfo in targetType.GetProperties())
                {
                    // 从源对象获取同名的属性信息
                    var sourcePropInfo = sourceType.GetProperty(targetPropInfo.Name);

                    Type sourcePropType = sourcePropInfo?.PropertyType;
                    Type targetPropType = targetPropInfo.PropertyType;

                    // 只在满足以下三个条件的情况下进行拷贝
                    // 1.源属性类型和目标属性类型一致
                    // 2.源属性可读
                    // 3.目标属性可写
                    if (sourcePropType == targetPropType
                        && sourcePropInfo.CanRead
                        && targetPropInfo.CanWrite)
                    {
                        // 获取属性值的表达式
                        Expression expression = Expression.Property(paramSourceExpr, sourcePropInfo);
                        Expression targetPropExpr = Expression.Property(paramTargetExpr, targetPropInfo);

                        // 如果目标属性是值类型或者字符串，则直接做赋值处理
                        // 暂不考虑目标值类型有非字符串的引用类型这种特殊情况
                        if (Utils.IsRefTypeExceptString(targetPropType))
                        {
                            expression = Expression.Call(null,
                                GetCopyMethodInfo(sourcePropType, targetPropType), expression);
                        }
                        binaryExpressions.Add(Expression.Assign(targetPropExpr, expression));
                    }
                }

                Expression bodyExpr = Expression.Block(binaryExpressions);

                var lambdaExpr
                    = Expression.Lambda<Action<TSource, TTarget>>(bodyExpr, paramSourceExpr, paramTargetExpr);

                _copyAction = lambdaExpr.Compile();
                _copyAction(source, target);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">源对象实例</param>
        /// <param name="target">目标对象实例</param>
        /// <returns></returns>
        private static MethodInfo GetCopyMethodInfo(Type source, Type target)
            => typeof(Copier<,>).MakeGenericType(source, target).GetMethod(nameof(Copy), new[] { source });
    }

    /// <summary>
    /// 工具类
    /// </summary>
    internal static class Utils
    {
        private static readonly Type _typeString = typeof(string);

        private static readonly Type _typeIEnumerable = typeof(IEnumerable);

        private static readonly ConcurrentDictionary<Type, Func<object>> _ctors = new ConcurrentDictionary<Type, Func<object>>();

        /// <summary>
        /// 判断是否是string以外的引用类型
        /// </summary>
        /// <returns>True：是string以外的引用类型，False：不是string以外的引用类型</returns>
        public static bool IsRefTypeExceptString(Type type)
            => !type.IsValueType && type != _typeString;

        /// <summary>
        /// 判断是否是string以外的可遍历类型
        /// </summary>
        /// <returns>True：是string以外的可遍历类型，False：不是string以外的可遍历类型</returns>
        public static bool IsIEnumerableExceptString(Type type)
            => _typeIEnumerable.IsAssignableFrom(type) && type != _typeString;

        /// <summary>
        /// 创建指定类型实例
        /// </summary>
        /// <param name="type">类型信息</param>
        /// <returns>指定类型的实例</returns>
        public static object CreateNewInstance(Type type) =>
            _ctors.GetOrAdd(type,
               t => Expression.Lambda<Func<object>>(Expression.New(t)).Compile())();
    }

    /// <summary>
    /// 可遍历类型拷贝器
    /// </summary>
    internal class EnumerableCopier
    {
        private static readonly MethodInfo _copyArrayMethodInfo;

        private static readonly MethodInfo _copyICollectionMethodInfo;

        private static readonly Type _typeICollection = typeof(ICollection<>);
        /// <summary>
        /// 
        /// </summary>
        static EnumerableCopier()
        {
            Type type = typeof(EnumerableCopier);
            _copyArrayMethodInfo = type.GetMethod(nameof(CopyArray));
            _copyICollectionMethodInfo = type.GetMethod(nameof(CopyICollection));
        }

        /// <summary>
        /// 根据IEnumerable的实现类类型选择合适的拷贝方法
        /// </summary>
        /// <param name="type">IEnumerable的实现类类型</param>
        /// <returns>拷贝方法信息</returns>
        public static MethodInfo GetMethondInfo(Type type)
        {
            if (type.IsArray)
            {
                return _copyArrayMethodInfo.MakeGenericMethod(type.GetElementType());
            }
            else if (type.GetGenericArguments().Length > 0)
            {
                Type elementType = type.GetGenericArguments()[0];
                if (_typeICollection.MakeGenericType(elementType).IsAssignableFrom(type))
                {
                    return _copyICollectionMethodInfo.MakeGenericMethod(type, elementType);

                }
            }
            throw new Exception($"Type[{type.Name}] has not been supported yet.");
        }

        /// <summary>
        /// 拷贝List
        /// </summary>
        /// <typeparam name="T">源ICollection实现类类型</typeparam>
        /// <typeparam name="TElement">源ICollection元素类型</typeparam>
        /// <param name="source">源ICollection对象</param>
        /// <returns>深拷贝完成的ICollection对象</returns>
        public static T CopyICollection<T, TElement>(T source)
            where T : ICollection<TElement>
        {
            T result = (T)Utils.CreateNewInstance(source.GetType());

            if (Utils.IsRefTypeExceptString(typeof(TElement)))
            {
                foreach (TElement item in source)
                {
                    result.Add(Copier<TElement, TElement>.Copy(item));
                }
            }
            else
            {
                foreach (TElement item in source)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 拷贝数组
        /// </summary>
        /// <typeparam name="TElement">源数组元素类型</typeparam>
        /// <param name="source">源List</param>
        /// <returns>深拷贝完成的数组</returns>
        public static TElement[] CopyArray<TElement>(TElement[] source)
        {
            TElement[] result = new TElement[source.Length];
            if (Utils.IsRefTypeExceptString(typeof(TElement)))
            {
                for (int i = 0; i < source.Length; i++)
                {
                    result[i] = Copier<TElement, TElement>.Copy(source[i]);
                }
            }
            else
            {
                for (int i = 0; i < source.Length; i++)
                {
                    result[i] = source[i];
                }
            }
            return result;
        }
    }
}
