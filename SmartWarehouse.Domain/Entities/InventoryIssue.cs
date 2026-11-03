using SmartWarehouse.Domain.Enums;
using System;
using System.Collections.Generic;

namespace SmartWarehouse.Domain.Entities
{
    /// <summary>Phiếu xuất kho (header) — 1-N với InventoryIssueDetail</summary>
    public class InventoryIssue
    {
        public int Id { get; set; }

        public string IssueCode { get; set; } = string.Empty;

        public int WarehouseId { get; set; }

        public DateTime IssueDate { get; set; }

        public IssueStatus Status { get; set; } = IssueStatus.Draft;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Warehouse Warehouse { get; set; } = null!;

        public ICollection<InventoryIssueDetail> Details { get; set; }
            = new List<InventoryIssueDetail>();
    }
}
