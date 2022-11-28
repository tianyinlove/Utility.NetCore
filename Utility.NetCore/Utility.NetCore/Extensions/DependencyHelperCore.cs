using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Utility.Dependency;

namespace Utility.Dependency
{
    /// <summary>
    /// 程序集方法
    /// </summary>
    public static partial class DependencyHelper
    {
        private static string _assemblyNamePrefix;

        /// <summary>
        /// 判断是否是引用项目或者是自有emapp相关项目
        /// </summary>
        /// <param name="library"></param>
        /// <returns></returns>
        public static bool IsProject(this RuntimeLibrary library)
        {
            if (library == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(_assemblyNamePrefix))
            {
                _assemblyNamePrefix = Assembly.GetEntryAssembly().GetName().Name.Split('.')[0];
            }
            return string.Equals(library.Type, "project", StringComparison.OrdinalIgnoreCase)
                || Regex.IsMatch(library.Name, $"^({_assemblyNamePrefix})", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 查找程序集
        /// </summary>
        /// <param name="libraryFilter"></param>
        /// <returns></returns>
        public static List<Assembly> GetAssemblies(Func<RuntimeLibrary, bool> libraryFilter = null)
        {
            if (libraryFilter == null)
            {
                libraryFilter = IsProject;
            }

            return DependencyContext.Default.RuntimeLibraries
                .Where(d => libraryFilter(d))
                .Select(library => Assembly.Load(new AssemblyName(library.Name)))
                .ToList();
        }

        /// <summary>
        /// 查找全部接口和对应的实现
        /// </summary>
        /// <returns></returns>
        /// <param name="libraryFilter">筛选dll</param>
        /// <param name="typeFilter">筛选接口和实现类</param>
        public static List<ServiceDescriptor> FindServices(Func<RuntimeLibrary, bool> libraryFilter = null, Func<Type, bool> typeFilter = null)
        {
            var libraries = GetAssemblies(libraryFilter);
            var (interfaces, classes) = FindTypes(libraries, typeFilter);
            var mappings = GetServiceMap(interfaces, classes, typeFilter);
            return mappings.Select(m => ServiceDescriptor.Describe(m.Service, m.Implementation, m.Implementation.GetServiceLife())).ToList();
        }

        /// <summary>
        /// 根据特性设置ioc周期
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ServiceLifetime GetServiceLife(this Type type)
        {
            var result = ServiceLifetime.Transient;
            if (type.GetCustomAttribute<ServiceLifeAttribute>() is ServiceLifeAttribute attr)
            {
                switch (attr.Mode)
                {
                    case ServiceLifeMode.Transient:
                        result = ServiceLifetime.Transient;
                        break;
                    case ServiceLifeMode.Singleton:
                        result = ServiceLifetime.Singleton;
                        break;
                    case ServiceLifeMode.Scoped:
                        result = ServiceLifetime.Scoped;
                        break;
                    default:
                        result = ServiceLifetime.Transient;
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 找出全部接口和实现类
        /// </summary>
        /// <returns></returns>
        private static (List<Type> interfaces, List<Type> classes) FindTypes(List<Assembly> assemblies, Func<Type, bool> typeFilter)
        {
            HashSet<Type> interfaces = new HashSet<Type>();
            HashSet<Type> classes = new HashSet<Type>();

            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException te)
                {
                    types = te.Types;
                }
                types = types.Where(d => d != null && (typeFilter?.Invoke(d) ?? true)).ToArray();
                foreach (var type in types)
                {
                    if (type.IsInterface)
                    {
                        interfaces.Add(type);
                    }
                    else if (type.IsClass && !type.IsAbstract && type.GetInterfaces().Any())
                    {
                        classes.Add(type);
                    }
                }
            }
            return (interfaces.ToList(), classes.ToList());
        }

    }

}
