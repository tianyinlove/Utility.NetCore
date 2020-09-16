#if NET_STD
using Aliyun.Api.LogService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility.Configuration;
using Utility.Constants;
using Utility.NetLog;
using AliLogInfo = Aliyun.Api.LogService.Domain.Log.LogInfo;

namespace Utility.Queues
{
    /// <summary>
    /// 阿里云日志上传队列
    /// </summary>
    static class AliyunLogQueue
    {
        /// <summary>
        /// 
        /// </summary>
        public static ILogServiceClient Client { get; private set; }

        /// <summary>
        /// 当前生效的配置
        /// </summary>
        public static AliyunOptions _options = null;

        /// <summary>
        /// 数据通知
        /// </summary>
        private static readonly AutoResetEvent _signal = new AutoResetEvent(false);

        /// <summary>
        /// 每次处理的数据条数
        /// </summary>
        private static readonly int _pageSize = 100;

        /// <summary>
        /// 数据队列
        /// </summary>
        private static readonly ConcurrentQueue<AliLogInfo> _queue = new ConcurrentQueue<AliLogInfo>();

        /// <summary>
        /// 
        /// </summary>
        static AliyunLogQueue()
        {
            ReConfig();
            Task.Factory.StartNew(Worker, CancellationToken.None, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task Worker()
        {
            await Task.Yield();
            while (_signal.WaitOne())
            {
                while (true)
                {
                    try
                    {
                        var logs = new List<AliLogInfo>();
                        for (int i = 0; i < _pageSize && _queue.TryDequeue(out AliLogInfo log); i++)
                        {
                            var keys = log.Contents.Keys.ToList();
                            foreach (var key in keys)
                            {
                                if (log.Contents[key] == null)
                                {
                                    log.Contents.Remove(key);
                                }
                            }
                            logs.Add(log);
                        }
                        if (logs.Count > 0 && Client != null)
                        {
                            var response = await Client.PostLogStoreLogsAsync(_options.LogStoreName, new Aliyun.Api.LogService.Domain.Log.LogGroupInfo
                            {
                                Topic = "",
                                LogTags =
                                {

                                },
                                Logs = logs,
                                Source = LogConfigLoader.CurrentValue.ApplicationName ?? ""
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception err)
                    {
                        LogFileQueue.Enqueue(new LogInfo
                        {
                            Level = LogLevel.Warning,
                            Exception = err.ToString(),
                            Message = "日志上传阿里云异常"
                        });
                    }
                }
                await Task.Yield();
            }
        }

        /// <summary>
        /// 加入待传输队列
        /// </summary>
        public static void Enqueue(AliLogInfo log)
        {
            _queue.Enqueue(log);
            _signal.Set();
        }

        /// <summary>
        /// 创建新的客户端
        /// </summary>
        internal static void ReConfig()
        {
            var config = LogConfigLoader.CurrentValue.Aliyun;

            if (_options == null
                || _options.Endpoint != config.Endpoint
                || _options.ProjectName != config.ProjectName
                || _options.AccessKeyId != config.AccessKeyId
                || _options.AccessKeySecret != config.AccessKeySecret)
            {
                if (!string.IsNullOrWhiteSpace(config.Endpoint)
                    && !string.IsNullOrWhiteSpace(config.ProjectName)
                    && !string.IsNullOrWhiteSpace(config.AccessKeyId)
                    && !string.IsNullOrWhiteSpace(config.AccessKeySecret))
                {
                    Client = LogServiceClientBuilders.HttpBuilder
                                .Endpoint(config.Endpoint, config.ProjectName)
                                .UseProxy(config.UseProxy)
                                .Credential(config.AccessKeyId, config.AccessKeySecret)
                                .Build();
                }
                else
                {
                    Client = null;
                }
            }
            _options = config;

        }
    }
}

#endif