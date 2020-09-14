using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Abstractions
{
    /// <summary>
    /// 接口响应的状态消息
    /// </summary>
    public class ApiStatus
    {
        /// <summary>
        /// 接口状态
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// 提示信息[可选填项]（缺省可以null或空字符串，甚至不包含该字段）
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 服务端计算时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 数据状态（下次请求相同的接口时回传）
        /// </summary>
        public string ViewState { get; set; }

        /// <summary>
        /// 最小请求间隔(秒)
        /// </summary>
        public int MinInterval { get; set; } = 0;

        /// <summary>
        /// 请求标识（从请求获取并原样返回）
        /// </summary>
        public string Identifier { get; set; }
    }
}
