using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Utility.Model;
using Utility.NetCore.Extensions;

namespace Utility.NetCore
{
    /// <summary>
    /// 通用输出数据格式
    /// </summary>
    public class ApiResult : IActionResult
    {
        /// <summary>
        /// 响应的状态消息
        /// </summary>
        public ApiStatus Result { get; set; } = new ApiStatus();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var apiData = new ApiData
            {
                Result = Result
            };
            context.HttpContext.Response.ContentType = "application/json;charset=utf-8";
            await context.HttpContext.Response.WriteAsync(apiData.ToApiJson() ?? string.Empty);
        }
    }

    /// <summary>
    /// 通用输出数据格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 具体协议自行定义JSON的对象代入[可选填项]（缺省可以null或空对象{}，甚至不包含该字段）
        /// </summary>
        public T Detail { get; set; }

        /// <summary>
        /// 输出到客户端
        /// </summary>
        /// <param name="context"></param>
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var apiData = new ApiData<T>
            {
                Result = Result,
                Detail = Detail
            };

            context.HttpContext.Response.ContentType = "application/json;charset=utf-8";
            await context.HttpContext.Response.WriteAsync(apiData.ToApiJson() ?? string.Empty);
        }
    }
}
