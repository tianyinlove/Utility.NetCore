#if NET_STD
using Microsoft.AspNetCore.Http;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Configuration;
using Utility.Constants;
using Utility.Extensions;
using Utility.Queues;
using System.Web;

namespace Utility.NetLog
{
    /// <summary>
    /// 日志
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// 记录跟踪id
        /// </summary>
        private const string TraceKey = "Emapp-TraceId";
        /// <summary>
        /// 服务调用跟踪
        /// </summary>
        private const string ServiceTraceKey = "Emapp-ServiceTrace";

        /// <summary>
        /// 服务调用跟踪
        /// </summary>
        private const string MethodNameKey = "Emapp-MethodName";


#if NET_STD
        static HttpContextAccessor _contextAccessor = new HttpContextAccessor();
#endif

        private static string CreateTraceId()
        {
            return $"{DateTime.Now:yyMMddHHmmss}{Guid.NewGuid().ToString("n").Substring(0, 10)}";
        }

        /// <summary>
        /// 记录(非结构化)日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message">摘要</param>
        public static void WriteLog(LogLevel level, string message)
        {
            WriteLog(level: level, message: message, detail: null, exception: null, module: null);
        }

        /// <summary>
        /// 记录(非结构化)日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message">摘要</param>
        /// <param name="detail">详情</param>
        public static void WriteLog(LogLevel level, string message, object detail)
        {
            WriteLog(level: level, message: message, detail: detail, exception: null, module: null);
        }

        /// <summary>
        /// 记录(非结构化)日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message">摘要</param>
        /// <param name="exception">异常信息</param>
        public static void WriteLog(LogLevel level, string message, Exception exception)
        {
            WriteLog(level: level, message: message, detail: null, exception: exception, module: null);
        }



        /// <summary>
        /// 记录(非结构化)日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="detail">详情</param>
        /// <param name="exception">异常信息</param>
        /// <param name="module"></param>
        private static void WriteLog(LogLevel level, string message, object detail, Exception exception, string module)
        {
            try
            {
                var config = LogConfigLoader.CurrentValue;
                if (level < config.Text.MinLevel && level < config.Aliyun.MinLevel)
                {
                    return;
                }

                var log = new LogInfo()
                {
                    Level = level,
                    Module = module,
                    Message = message,
                    Time = DateTime.Now
                };
#if NET_STD
                var httpContext = _contextAccessor.HttpContext;
#else
                var httpContext = HttpContext.Current;
#endif
                if (httpContext?.Request != null)
                {
                    var request = httpContext.Request;

#if NET_STD
                    log.HttpMethod = request.Method;
                    log.IpAddress = request.Headers["X-Forwarded-For"];
                    if (!string.IsNullOrWhiteSpace(log.IpAddress))
                    {
                        log.IpAddress = log.IpAddress.Split(";,".ToArray(), StringSplitOptions.RemoveEmptyEntries).Last(); //取最后一个由前端负载返回的客户端ip
                    }
                    else
                    {
                        log.IpAddress = httpContext.Connection.RemoteIpAddress.ToString();
                    }

                    log.Url = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
#else   
                    
                    log.HttpMethod = request.HttpMethod;
                    log.IpAddress = request.Headers["X-Forwarded-For"];
                    if (!string.IsNullOrWhiteSpace(log.IpAddress))
                    {
                        log.IpAddress = log.IpAddress.Split(";,".ToArray(), StringSplitOptions.RemoveEmptyEntries).Last(); //取最后一个由前端负载返回的客户端ip
                    }
                    else
                    {
                        log.IpAddress = request.UserHostAddress;
                    }
                    log.Url = request.Url.ToString();
#endif

                    if (httpContext.Items[TraceKey] is string traceId)
                    {
                        log.TraceId = traceId;
                    }
                    else
                    {
                        log.TraceId = httpContext.Request.Headers[TraceKey];
                        if (string.IsNullOrWhiteSpace(log.TraceId))
                        {
                            log.TraceId = CreateTraceId();
                        }
                        httpContext.Items[TraceKey] = log.TraceId;
                    }
                    string track = httpContext.Request.Headers[ServiceTraceKey];
                    if (!string.IsNullOrWhiteSpace(track))
                    {
                        log.Track = track;
                    }
                    string methodfullname = httpContext.Request.Headers[MethodNameKey];
                    if (!string.IsNullOrWhiteSpace(methodfullname))
                    {
                        log.Method = methodfullname;
                    }

                }


                if (exception != null)
                {
                    log.Exception = exception.ToString();
                }

                if (detail != null)
                {
                    if (detail is string)
                    {
                        log.Detail = detail as string;
                    }
                    else if (detail.GetType().IsValueType)
                    {
                        log.Detail = detail.ToString();
                    }
                    else
                    {
                        log.Detail = detail.ToJson();
                    }

                }
                if (level >= config.Text.MinLevel)
                {
                    LogFileQueue.Enqueue(log);
                }
#if NET_STD

                if (level >= config.Aliyun.MinLevel)
                {
                    var aliLog = new Aliyun.Api.LogService.Domain.Log.LogInfo
                    {
                        Time = DateTimeOffset.Now,
                        Contents = {
                                { "Level",log.Level.ToString()},
                                { "Module",log.Module},
                                { "Message",log.Message},
                                { "Detail",log.Detail},
                                { "Exception",log.Exception}
                            }
                    };
                    //aliLog.Contents["AppName"] = config.ApplicationName;
                    if (httpContext != null)
                    {
                        aliLog.Contents["TraceId"] = log.TraceId;
                        aliLog.Contents["Track"] = log.Track;
                        aliLog.Contents["Method"] = log.Method;
                        aliLog.Contents["Url"] = log.Url;
                        aliLog.Contents["HttpMethod"] = log.HttpMethod;
                        aliLog.Contents["Ip"] = log.IpAddress;
                    }
                    AliyunLogQueue.Enqueue(aliLog);
                }
#endif
            }
            catch
            {
            }
        }

        /// <summary>
        /// 记录(非结构化)日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="detail">详情</param>
        /// <param name="exception">异常信息</param>
        public static void WriteLog(LogLevel level, string message, object detail, Exception exception)
        {
            WriteLog(level: level, message: message, detail: detail, exception: exception, module: null);
        }
    }
}
