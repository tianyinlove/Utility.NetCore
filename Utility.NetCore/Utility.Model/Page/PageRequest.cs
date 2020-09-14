using System;
using System.Collections.Generic;
using System.Text;
using Utility.Model.Constants;

namespace Utility.Model.Page
{
    /// <summary>
    /// 分页参数
    /// </summary>
    public abstract class PageRequest
    {
        /// <summary>
        /// 排序方式,可以为多个排序
        /// </summary>
        public List<SortSelector> SortBy { get; set; }

        /// <summary>
        /// 翻页大小
        /// </summary>
        private int _pageSize = 0;

        /// <summary>
        /// 分页大小默认值
        /// </summary>
        private int _defaultPageSize = 10;

        /// <summary>
        /// 翻页大小
        /// </summary>
        public int PageSize
        {
            get => _pageSize <= 0 ? _defaultPageSize : _pageSize;
            set => _pageSize = value;
        }

        /// <summary>
        /// 分页大小默认值
        /// </summary>
        public void SetDefaultPageSize(int defaultPageSize)
        {
            if (defaultPageSize > 0)
            {
                _defaultPageSize = defaultPageSize;
            }
        }
    }
}
