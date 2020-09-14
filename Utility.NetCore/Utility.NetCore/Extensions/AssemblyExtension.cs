using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utility.NetCore.Extensions
{
    /// <summary>
    /// 程序集扩展
    /// </summary>
    public static class AssemblyExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAssembly(this Assembly assembly, IServiceCollection services)
        {
            AppDomain.CurrentDomain.GetAssemblies().Where(o => o.Equals(assembly)).ToList().ForEach(o =>
            {
                o.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.IsSealed).ToList().ForEach(t =>
                {
                    var iface = t.GetInterfaces();
                    if (iface != null && iface.Length > 0)
                    {
                        var ifaceLst = iface.ToList();
                        ifaceLst.ForEach(It =>
                        {
                            if (It != null)
                            {
                                services.AddTransient(It, t);
                            }
                        });
                    }
                });
            });
            return services;
        }
    }
}
