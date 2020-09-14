using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Utility.Abstractions
{
    /// <summary>
    /// api运行异常
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// 抛出api异常并发送到接口输出
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ApiException(int code, string message) : base(message)
        {
            StatusCode = code;
        }

        /// <summary>
        /// 抛出http异常
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ApiException(HttpStatusCode code, string message) : base(message)
        {
            StatusCode = code == HttpStatusCode.OK ? 0 : (int)code;
        }

        /// <summary>
        /// 是否自动保存日志
        /// </summary>
        public bool AutoLog { get; set; } = false;

        /// <summary>
        /// 接口状态码
        /// </summary>
        public int StatusCode { get; set; } = 0;
    }
}
