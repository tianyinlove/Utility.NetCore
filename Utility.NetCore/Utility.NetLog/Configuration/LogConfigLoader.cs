using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Utility.Extensions;

namespace Utility.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    static class LogConfigLoader
    {
        /// <summary>
        /// App_Data
        /// </summary>
        private static string AppdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

        /// <summary>
        /// 配置文件全路径
        /// </summary>
        private static readonly string _configFileFullName = Path.Combine(AppdataPath, "log.json");

        /// <summary>
        /// 文件监视
        /// </summary>
        private static FileSystemWatcher _watcher;

        /// <summary>
        /// 文件数据
        /// </summary>
        public static LogConfig CurrentValue { get; private set; }

        /// <summary>
        /// 修改后到生效的延迟时间
        /// </summary>
        private static readonly int _delayMillisecond = 1000;

        /// <summary>
        /// 读取配置文件线程定时器
        /// </summary>
        private static Timer _timer;

        /// <summary>
        /// 
        /// </summary>
        private static bool _configIsChanged = false;

        /// <summary>
        /// 
        /// </summary>
        static LogConfigLoader()
        {
            try
            {
                if (!Directory.Exists(AppdataPath))
                {
                    Directory.CreateDirectory(AppdataPath);
                }
            }
            catch
            {
            }

            LoadConfig();

            string filename = Path.GetFileName(_configFileFullName);
            _watcher = new FileSystemWatcher(AppdataPath, filename);
            _watcher.Changed += ConfigFileWatcher_Changed;
            _watcher.EnableRaisingEvents = true;
            _timer = new Timer(new TimerCallback(OnConfigFileChanged), null, _delayMillisecond, Timeout.Infinite);
        }

        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="state"></param>
        private static void OnConfigFileChanged(object state)
        {
            if (_configIsChanged)
            {
                _configIsChanged = false;
                LoadConfig();
            }
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private static void LoadConfig()
        {
            try
            {
                var configJson = "";
                if (File.Exists(_configFileFullName))
                {
                    configJson = File.ReadAllText(_configFileFullName, Encoding.UTF8);
                }
                if (!string.IsNullOrEmpty(configJson))
                {
                    CurrentValue = configJson.FromJson<LogConfig>();
                }
                else
                {
                    CurrentValue = new LogConfig();
                    configJson = CurrentValue.ToJson();
                    File.WriteAllText(_configFileFullName, configJson, Encoding.UTF8);
                }
            }
            catch
            {
                CurrentValue = new LogConfig();
            }
            if (string.IsNullOrWhiteSpace(CurrentValue.ApplicationName))
            {
                CurrentValue.ApplicationName = Assembly.GetEntryAssembly().GetName().Name;
            }
        }

        /// <summary>
        /// 文件有修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConfigFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _configIsChanged = true;
            _timer.Change(_delayMillisecond, Timeout.Infinite);
        }
    }
}
