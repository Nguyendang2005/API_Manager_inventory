using SmartWarehouse.Domain.Enums;
using System;
using System.Collections.Generic;

namespace SmartWarehouse.Domain.Entities
{
    /// <summary>Phiếu nhập kho (header) — 1-N với InventoryReceiptDetail</summary>
    public class InventoryReceipt
    {
        public int Id { get; set; }

        // Mã phiếu duy nhất, ví dụ: RCP-20261030-001
        public string ReceiptCode { get; set; } = string.Empty;

        public int WarehouseId { get; set; }

        public int SupplierId { get; set; }

        // Ngày nhận hàng thực tế
        public DateTime ReceivedDate { get; set; }

        public ReceiptStatus Status { get; set; } = ReceiptStatus.Draft;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Warehouse Warehouse { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;

        public ICollection<InventoryReceiptDetail> Details { get; set; }
            = new List<InventoryReceiptDetail>();
    }
}
