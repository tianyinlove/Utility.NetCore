using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Utility.Configuration
{
    /// <summary>
    /// 配置加载器
    /// </summary>
    public abstract class FileWatcher<TContent>
    {
        /// <summary>
        /// 修改后到生效的延迟时间
        /// </summary>
        private readonly int _delayMillisecond;

        /// <summary>
        /// 解析配置文件
        /// </summary>
        private readonly Func<string, TContent> _deserializer;

        /// <summary>
        /// 默认配置
        /// </summary>
        private readonly Func<TContent> _getDefault;

        /// <summary>
        /// 配置文件全路径
        /// </summary>
        private readonly string _configFileFullName;

        /// <summary>
        /// 文件监视
        /// </summary>
        private FileSystemWatcher _watcher;

        /// <summary>
        /// 文件数据
        /// </summary>
        public TContent Value { get; private set; }


        /// <summary>
        /// 读取配置文件线程定时器
        /// </summary>
        private readonly Timer _timer;

        private bool _configIsChanged = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileFullName">配置文件完整路径</param>
        /// <param name="deserializer">解析配置文件</param>
        /// <param name="getDefault">创建默认配置</param>
        /// <param name="delayMillisecond">配置文件修改后的延迟生效时间</param>
        public FileWatcher(string fileFullName, Func<string, TContent> deserializer, Func<TContent> getDefault, int delayMillisecond = 2000)
        {
            _delayMillisecond = delayMillisecond > 0 ? delayMillisecond : 2000;
            _configFileFullName = fileFullName ?? throw new ArgumentNullException(nameof(fileFullName));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            _getDefault = getDefault ?? throw new ArgumentNullException(nameof(getDefault));
            Value = getDefault();
            LoadConfig();

            string path = Path.GetDirectoryName(fileFullName);
            string filename = Path.GetFileName(fileFullName);
            if (Directory.Exists(path))
            {
                _watcher = new FileSystemWatcher(path, filename);
                _watcher.Changed += ConfigFileWatcher_Changed;
                _watcher.Created += ConfigFileWatcher_Changed;
                _watcher.Deleted += ConfigFileWatcher_Changed;
                _watcher.EnableRaisingEvents = true;
                _timer = new Timer(new TimerCallback(OnConfigFileChanged), null, delayMillisecond, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="state"></param>
        private void OnConfigFileChanged(object state)
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
        private void LoadConfig()
        {
            try
            {
                if (File.Exists(_configFileFullName))
                {
                    Value = _deserializer(File.ReadAllText(_configFileFullName, Encoding.UTF8));
                }
                else
                {
                    Value = _getDefault();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 文件有修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _configIsChanged = true;
            _timer.Change(_delayMillisecond, Timeout.Infinite);
        }
    }
}
