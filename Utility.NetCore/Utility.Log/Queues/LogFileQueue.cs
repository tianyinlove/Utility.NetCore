using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Utility.Configuration;
using Utility.Constants;
using Utility.Log;

namespace Utility.Queues
{
    /// <summary>
    /// 
    /// </summary>
    static class LogFileQueue
    {
        /// <summary>
        /// 数据通知
        /// </summary>
        private static readonly AutoResetEvent _signal = new AutoResetEvent(false);

        /// <summary>
        /// 每次处理的数据条数
        /// </summary>
        private const int _pageSize = 100;

        /// <summary>
        /// 数据队列
        /// </summary>
        private static readonly Dictionary<LogLevel, ConcurrentQueue<LogInfo>> _queues = new Dictionary<LogLevel, ConcurrentQueue<LogInfo>>();

        /// <summary>
        /// 日志文件拆分时间
        /// </summary>
        private static DateTime _lastLoopTime = DateTime.Now.AddDays(-1);

        /// <summary>
        /// 
        /// </summary>
        static LogFileQueue()
        {
            for (LogLevel level = LogLevel.Trace; level <= LogLevel.Critical; level++)
            {
                _queues[level] = new ConcurrentQueue<LogInfo>();
            }
            new Thread(Worker) { IsBackground = true }.Start();
        }

        /// <summary>
        /// 插入待处理数据
        /// </summary>
        /// <param name="info"></param>
        public static void Enqueue(LogInfo info)
        {
            _queues[info.Level].Enqueue(info);
            _signal.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private static void Worker(object obj)
        {
            while (_signal.WaitOne())
            {
                try
                {
                    var config = LogConfigLoader.CurrentValue.Text;
                    for (LogLevel level = LogLevel.Trace; level <= LogLevel.Critical; level++)
                    {
                        while (true)
                        {
                            StringBuilder buff = new StringBuilder();
                            for (int i = 0; i < _pageSize && _queues[level].TryDequeue(out LogInfo log); i++)
                            {
                                buff.AppendLine($@"[{log.Time:yyyy-MM-dd HH:mm:ss.fff}] {log.Module}");
                                if (log.Url != null)
                                {
                                    buff.AppendLine($"{log.HttpMethod} {log.Url} {log.IpAddress}");
                                }
                                if (log.Message != null)
                                {
                                    buff.AppendLine(log.Message);
                                }
                                if (log.Detail != null)
                                {
                                    buff.AppendLine(log.Detail);
                                }
                                if (log.Exception != null)
                                {
                                    buff.AppendLine($@"{log.Exception}");
                                }
                            }
                            if (buff.Length > 0)
                            {
                                var time = DateTime.Now;
                                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.SavePath.Trim("\\/".ToArray()), level.ToString());  //日志文件夹
                                string basename = Path.Combine(path, $"{time:yyyyMMdd}");
                                string logFilePathName = Path.Combine(path, $"{time:yyyyMMdd}.log");  //日志文件

                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                if (File.Exists(logFilePathName))
                                {
                                    File.AppendAllText(logFilePathName, buff.ToString(), Encoding.UTF8);
                                    if (_lastLoopTime < DateTime.Now.AddSeconds(-5))    //每隔5秒钟检查是否拆分日志文件
                                    {
                                        if (new FileInfo($"{basename}.log").Length >= config.MaxFileSize)  //文件拆分
                                        {
                                            if (File.Exists($"{basename}-{config.MaxLoopCount}.log"))
                                            {
                                                File.Delete($"{basename}-{config.MaxLoopCount}.log");
                                            }
                                            for (int loop = config.MaxLoopCount - 1; loop > 0; loop--)
                                            {
                                                if (File.Exists($"{basename}-{loop}.log"))
                                                {
                                                    File.Move($"{basename}-{loop}.log", $"{basename}-{loop + 1}.log");
                                                }
                                            }
                                            File.Move($"{basename}.log", $"{basename}-1.log");
                                        }
                                        _lastLoopTime = DateTime.Now;
                                    }
                                }
                                else
                                {
                                    File.AppendAllText(logFilePathName, buff.ToString(), Encoding.UTF8);
                                    var logs = Directory.GetFiles(path, "*.log").OrderByDescending(d => d).ToList();
                                    if (logs.Count > config.MaxFileCount)  //删除历史文件
                                    {
                                        for (int i = config.MaxFileCount; i < logs.Count; i++)
                                        {
                                            File.Delete(logs[i]);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}
