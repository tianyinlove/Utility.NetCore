using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Abstractions.Page
{
    /// <summary>
    /// 按页码分页的结果
    /// </summary>
    public class IndexPageResult<T>
    {
        /// <summary>
        /// 分页数据
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 当前页码(从0开始)
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 翻页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 最大页码(从0开始)
        /// </summary>
        public int MaxPage =>
            Total <= 0 || PageSize <= 0 ? 0
            : Total % PageSize == 0 ? Total / PageSize - 1
            : Total / PageSize;
    }
}
