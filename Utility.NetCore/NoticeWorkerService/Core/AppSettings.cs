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
        public List<string> NameList { get; set; }
    }
}
