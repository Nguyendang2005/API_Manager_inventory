using System;
using System.Collections.Generic;

namespace SmartWarehouse.Domain.Entities
{
    /// <summary>
    /// Warehouse facility entity (Code, Name, Address, etc.)
    /// </summary>
    public class Warehouse
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<WarehouseLocation> Locations { get; set; } = new List<WarehouseLocation>();
        public ICollection<InventoryReceipt> InventoryReceipts { get; set; } = new List<InventoryReceipt>();
        public ICollection<InventoryIssue> InventoryIssues { get; set; } = new List<InventoryIssue>();
    }
}
