using System;

namespace SmartWarehouse.Application.DTOs.InventoryReceipt
{
    public class ReceiptMonthlySummaryDto
    {
        public int WarehouseId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalReceipts { get; set; }
        public int TotalItemsReceived { get; set; }
        public decimal TotalCost { get; set; }
    }
}
