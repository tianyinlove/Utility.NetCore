using Microsoft.Extensions.Hosting;
using NoticeWorkerService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utility.Constants;
using Utility.NetLog;

namespace NoticeWorkerService
{
    /// <summary>
    /// 
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly IStockMonitorService _stockMonitorService;

        /// <summary>
        /// 
        /// </summary>
        public Worker(IStockMonitorService stockMonitorService)
        {
            _stockMonitorService = stockMonitorService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _stockMonitorService.SendMessage();
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(LogLevel.Error, "服务异常", ex);
                }
                await Task.Delay(5 * 1000, stoppingToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.WriteLog(LogLevel.Info, "启动服务");
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.WriteLog(LogLevel.Info, "关闭服务");
            return base.StopAsync(cancellationToken);
        }
    }
}
