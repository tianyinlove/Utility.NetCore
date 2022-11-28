using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utility.Dependency;
using Utility.Middlewares;

namespace Utility.Extensions
{
    /// <summary>
    /// 默认异常处理
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 自动添加程序集合中的接口和实现类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="libraryFilter">筛选程序集</param>
        /// <param name="typeFilter">筛选接口和实现类</param>
        /// <param name="selector">有多个实现类的时候，选择一个指定实现类。选择器可以返回空值，sdk会选择一个最优实现类</param>
        /// <returns></returns>
        public static IServiceCollection AddProductService(
            this IServiceCollection services,
            Func<RuntimeLibrary, bool> libraryFilter = null,
            Func<Type, bool> typeFilter = null,
            Func<IEnumerable<ServiceDescriptor>, ServiceDescriptor> selector = null)
        {
            var serviceDescriptors = DependencyHelper
                .FindServices(libraryFilter, typeFilter)
                .GroupBy(d => d.ServiceType)
                .Select(mappings => selector?.Invoke(mappings) ?? DefaultServiceDescriptorSelector(mappings))
                .ToList();

            foreach (var serviceDescriptor in serviceDescriptors)
            {
                services.TryAdd(serviceDescriptor);
            }
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseException(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<ExceptionMiddleware>();
        }

        private static string _prefix;

        /// <summary>
        /// 按接口和类的继承次数选择最近的一个实现类
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        static ServiceDescriptor DefaultServiceDescriptorSelector(IEnumerable<ServiceDescriptor> services)
        {
            if (string.IsNullOrEmpty(_prefix))
            {
                _prefix = Assembly.GetEntryAssembly().GetName().Name.Split('.')[0] + "."; // 计算入口项目namespace前缀
            }
            return services.OrderBy(descriptor =>
            {
                var order = GetDistance(descriptor.ImplementationType, descriptor.ServiceType);
                if (!descriptor.ImplementationType.FullName.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
                {
                    order += 1000;
                }
                return order;
            }).FirstOrDefault();
        }

        /// <summary>
        /// 计算接口和实现的距离，在接口出现继承时，总是选择一个最近的实现类
        /// </summary>
        /// <param name="impl"></param>
        /// <param name="interface"></param>
        /// <returns></returns>
        static int GetDistance(Type impl, Type @interface)
        {
            int distance = 1;
            var baseType = impl.BaseType;
            while (baseType != null
                && baseType != typeof(object)
                && !baseType.IsAbstract && @interface.IsAssignableFrom(baseType))
            {
                distance++;
                baseType = baseType.BaseType;
            }

            var maxDistince = impl.GetInterfaces()
                                  .Where(d => d != @interface && @interface.IsAssignableFrom(d))
                                  .Select(d => GetDistance(d, @interface))
                                  .OrderByDescending(d => d)
                                  .FirstOrDefault();
            distance += maxDistince * 5;
            return distance;
        }
    }
}
