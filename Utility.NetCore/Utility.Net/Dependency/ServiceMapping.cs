using System;

namespace Utility.Dependency
{
	/// <summary>
	/// 接口和实现类的对应关系
	/// </summary>
	public class ServiceMapping
	{
		/// <summary>
		/// 接口类型
		/// </summary>
		public Type Service { get; set; }

        /// <summary>
        /// 实现类
        /// </summary>
        public Type Implementation { get; set; }

		/// <summary>
		/// 生命周期
		/// </summary>
		public ServiceLifeMode Mode { get; set; }
	}
}
