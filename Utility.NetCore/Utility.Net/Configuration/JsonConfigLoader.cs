using Newtonsoft.Json;

namespace Utility.Configuration
{
    /// <summary>
    /// 配置加载器
    /// </summary>
    /// <typeparam name="TConfig">配置文件的类型</typeparam>
    public class JsonConfigLoader<TConfig> : FileWatcher<TConfig>
        where TConfig : class, new()
    {
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <returns></returns>
        public TConfig Get()
        {
            return Value;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFileFullName">配置文件完整路径</param>
        /// <param name="timeoutMillisecond">配置文件修改后的延迟生效时间</param>
        public JsonConfigLoader(string configFileFullName, int timeoutMillisecond = 2000)
            : base(configFileFullName, json => JsonConvert.DeserializeObject<TConfig>(json), () => new TConfig(), timeoutMillisecond)
        {
        }

    }
}
