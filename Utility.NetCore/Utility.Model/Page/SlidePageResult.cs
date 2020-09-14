using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Model.Page
{
    /// <summary>
    /// 简单的分页列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SlidePageResult<T>
    {
        /// <summary>
        /// 普通列表
        /// </summary>
        public List<T> List { get; set; } = new List<T>();

        /// <summary>
        /// 是否清空历史数据
        /// </summary>
        public bool Flush { get; set; } = false;

        /// <summary>
        /// 是否结束翻页
        /// </summary>
        public bool End { get; set; } = false;
    }
}
