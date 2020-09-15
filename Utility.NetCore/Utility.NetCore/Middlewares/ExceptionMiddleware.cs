using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utility.Model;
using Utility.Extensions;
using Utility.Log;

namespace Utility.Middlewares
{
    /// <summary>
    /// 默认异常处理
    /// </summary>
    internal class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (ApiException ex)
            {
                var result = new ApiData
                {
                    Result = new ApiStatus
                    {
                        Code = ex.StatusCode,
                        Msg = ex.Message
                    }
                };
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(result.ToApiJson() ?? string.Empty);
            }
            catch (Exception ex)
            {
                Logger.Error("服务器异常", ex);
                var result = new ApiData
                {
                    Result = new ApiStatus
                    {
                        Code = -1,
                        Msg = "服务器异常"
                    }
                };
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(result.ToApiJson() ?? string.Empty);
            }
        }
    }
}
