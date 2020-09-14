using System;
using System.Collections.Generic;
using System.Text;
using Utility.Abstractions.Constants;

namespace Utility.Abstractions.Page
{
    /// <summary>
    /// 分页参数
    /// </summary>
    public class SlidePageRequest : PageRequest
    {
        /// <summary>
        /// 翻页方向
        /// </summary>
        public Direction Direction { get; set; } = Direction.ReNew;
    }
}
