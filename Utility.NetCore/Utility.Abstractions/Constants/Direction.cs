using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Abstractions.Constants
{
    /// <summary>
    /// 列表翻页方向
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// 向上/刷新
        /// </summary>
        Up = 1,

        /// <summary>
        /// 向下/更多
        /// </summary>
        Down = 2,

        /// <summary>
        /// 取新数据
        /// </summary>
        ReNew = 0
    }
}
