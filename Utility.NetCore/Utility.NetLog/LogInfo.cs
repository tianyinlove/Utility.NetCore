using System;
using System.Collections.Generic;
using System.Text;
using Utility.Constants;

namespace Utility.NetLog
{
    /// <summary>
    /// 日志信息
    /// </summary>
    public class LogInfo
    {
        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// 事件追踪id
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// 服务调用路径
        /// </summary>
        public string Track { get; set; }

        /// <summary>
        /// 服务方法名
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 程序所在模块
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 日志摘要
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 日志详情
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 异常详情
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 日志记录时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 上下文url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// 客户端ip
        /// </summary>
        public string IpAddress { get; set; }
    }
}
