using System;

namespace Utility.Dependency
{
    /// <summary>
    /// 生命周期管理
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ServiceLifeAttribute : Attribute
    {
        /// <summary>
        /// 实例化的周期类型
        /// </summary>
        public ServiceLifeMode Mode { get; }

        /// <summary>
        /// 
        /// </summary>
        public ServiceLifeAttribute(ServiceLifeMode mode = ServiceLifeMode.Transient)
        {
            Mode = mode;
        }

    }
}
