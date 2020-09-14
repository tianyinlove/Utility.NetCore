using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Abstractions.Page
{
    /// <summary>
    /// 按页码分页参数
    /// </summary>
    public class IndexPageRequest : PageRequest
    {
        /// <summary>
        /// 当前页码，从0开始
        /// </summary>
        public int PageIndex { get; set; } = 0;
    }
}
