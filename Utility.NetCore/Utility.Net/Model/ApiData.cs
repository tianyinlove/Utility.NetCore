using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Model
{
    /// <summary>
    /// Api接口数据
    /// </summary>
    public class ApiData
    {
        /// <summary>
        /// 响应的状态消息
        /// </summary>
        public ApiStatus Result { get; set; } = new ApiStatus();
    }

    /// <summary>
    /// Api接口数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiData<T> : ApiData
    {
        /// <summary>
        /// 具体协议自行定义JSON的对象代入[可选填项]（缺省可以null或空对象{}，甚至不包含该字段）
        /// </summary>
        public T Detail { get; set; }
    }
}
