using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility.Dependency
{
    /// <summary>
    /// 查找接口和实现
    /// </summary>
    public static partial class DependencyHelper
    {
        private static ServiceLifeMode GetLifeMode(this Type type)
        {
            return type.GetCustomAttribute<ServiceLifeAttribute>()?.Mode ?? ServiceLifeMode.Transient;
        }

        /// <summary>
        /// 从程序集中查找所有接口和实现类
        /// </summary>
        /// <param name="interfaces"></param>
        /// <param name="classes"></param>
        /// <param name="typeFilter">筛选接口和实现类</param>
        /// <returns></returns>
        private static List<ServiceMapping> GetServiceMap(List<Type> interfaces, List<Type> classes, Func<Type, bool> typeFilter)
        {
            var result = new List<ServiceMapping>();

            foreach (var service in interfaces)
            {
                if (!service.IsGenericType)
                {
                    foreach (var impl in classes)
                    {
                        if (!impl.IsGenericType && service.IsAssignableFrom(impl))
                        {
                            result.Add(new ServiceMapping { Service = service, Implementation = impl, Mode = impl.GetLifeMode() });
                        }
                    }
                }
                else
                {
                    List<Type> genericImpls = new List<Type>();
                    var genericMap = new List<ServiceMapping>();

                    // 泛型实现类，并且类型参数数量一致的时候，使用泛型注入
                    // 例如 Service<T>: IService<T>
                    foreach (var genericImpl in classes)
                    {
                        if (genericImpl.IsGenericType
                            && genericImpl.GenericTypeArguments.Length == service.GenericTypeArguments.Length
                            && genericImpl.GetInterfaces().Any(dd => dd.IsGenericType && dd.GetGenericTypeDefinition() == service))
                        {
                            genericImpls.Add(genericImpl);
                            genericMap.Add(new ServiceMapping
                            {
                                Service = service,
                                Implementation = genericImpl,
                                Mode = genericImpl.GetLifeMode()
                            });
                        }
                    }

                    // 非泛型实现类按特定类型参数注入
                    // 例如 IntService : IService<int>                    
                    foreach (var typeImpl in classes)
                    {
                        if (typeImpl.IsGenericType || !typeImpl.GetInterfaces().Any(dd => dd.IsGenericType && dd.GetGenericTypeDefinition() == service))
                        {
                            continue;
                        }
                        foreach (var typeInterface in typeImpl.GetInterfaces())
                        {
                            if (typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == service)
                            {
                                result.Add(new ServiceMapping
                                {
                                    Service = typeInterface,
                                    Implementation = typeImpl,
                                    Mode = typeImpl.GetLifeMode()
                                });

                                foreach (var impl in genericImpls)
                                {
                                    if (impl.GetGenericArguments().Length == typeInterface.GetGenericArguments().Length)
                                    {
                                        genericMap.Add(new ServiceMapping
                                        {
                                            Service = typeInterface,
                                            Implementation = impl.MakeGenericType(typeInterface.GetGenericArguments()),
                                            Mode = typeImpl.GetLifeMode()
                                        });
                                    }
                                }
                            }
                        }
                    }
                    result.AddRange(genericMap);
                }
            }

            return result;
        }

    }
}
