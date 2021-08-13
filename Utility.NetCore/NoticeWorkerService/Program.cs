using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NoticeWorkerService.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Utility.Extensions;

namespace NoticeWorkerService
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    var config = new ConfigurationBuilder()
                        .SetBasePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config"))
                        .AddJsonFile("appsettings.json", optional: true)
                        .Build();
                    services.Configure<AppSettings>(config);
                    var _assembly = Assembly.GetExecutingAssembly();
                    services.AddAssembly(_assembly);
                    services.AddHttpClient();
                    services.AddMemoryCache();
                    services.AddHostedService<Worker>();
                });
    }
}
