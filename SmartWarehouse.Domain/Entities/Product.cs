using System;
using System.Collections.Generic;

namespace SmartWarehouse.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Unit { get; set; } = "Piece";

        public decimal UnitWeightKg { get; set; }

        public decimal UnitPrice { get; set; }

        // Số lượng tồn kho hiện tại
        public int StockQuantity { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Category Category { get; set; } = null!;
        public ICollection<InventoryReceiptDetail> ReceiptDetails { get; set; } = new List<InventoryReceiptDetail>();
        public ICollection<InventoryIssueDetail> IssueDetails { get; set; } = new List<InventoryIssueDetail>();
    }
}
