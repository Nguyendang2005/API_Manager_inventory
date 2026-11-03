using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartWarehouse.Application.Common
{
    public class PaginationParams
    {
        [Range(1,int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string SortBy { get; set; } = "id";
        public string SortDirection { get; set; } = "asc";

    }
}
