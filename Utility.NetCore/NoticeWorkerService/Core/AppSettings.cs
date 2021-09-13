using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoticeWorkerService.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public int StartHour { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int EndHour { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TradeUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<StockPoolConfig> StockPoolList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StockPoolConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 接收标签ID，多个用|分隔，最多支持1000个
        /// </summary>
        public string ToTag { get; set; }
        /// <summary>
        /// 接收成员ID，多个用|分隔，最多支持1000个
        /// </summary>
        public string ToUser { get; set; }
        /// <summary>
        /// 接收部门ID，多个用|分隔，最多支持1000个
        /// </summary>
        public string ToParty { get; set; }
    }
}
