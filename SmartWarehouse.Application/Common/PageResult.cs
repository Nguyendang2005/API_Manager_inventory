using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWarehouse.Application.Common
{
    public class PageResult<T>
    {

        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    }
}
