using System;
using System.Collections.Generic;
using System.Text;
using Utility.Abstractions.Constants;

namespace Utility.Abstractions.Page
{
    /// <summary>
    /// 排序方式
    /// </summary>
    public class SortSelector
    {
        /// <summary>
        /// 排序关键字
        /// </summary>
        public string SortKey { get; set; }

        /// <summary>
        /// 排序类型
        /// </summary>
        public SortType SortType { get; set; } = SortType.None;
    }
}
