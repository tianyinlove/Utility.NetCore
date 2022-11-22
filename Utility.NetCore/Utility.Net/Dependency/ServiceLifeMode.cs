namespace Utility.Dependency
{
    /// <summary>
    /// 实例化的方式
    /// </summary>
    public enum ServiceLifeMode
    {
        /// <summary>
        /// 每次创建新实例
        /// </summary>
        Transient = 1,

        /// <summary>
        /// 单实例
        /// </summary>
        Singleton = 2,

        /// <summary>
        /// 一个作用域内单实例,一个Scope代表一个线程内部,在 ASP.NET Core表示一次浏览器请求作用域
        /// </summary>
        Scoped = 3
    }
}
