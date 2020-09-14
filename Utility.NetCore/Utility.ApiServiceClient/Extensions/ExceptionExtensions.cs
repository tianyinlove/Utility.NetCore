using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using Utility.ApiServiceClient.Middlewares;

namespace Utility.ApiServiceClient.Extensions
{
    /// <summary>
    /// 默认异常处理
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseException(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
