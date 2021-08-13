using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoticeWorkerService.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStockMonitorService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task SendMessage();
    }
}
