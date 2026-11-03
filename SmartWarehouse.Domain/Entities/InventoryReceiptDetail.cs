namespace SmartWarehouse.Domain.Entities
{
    /// <summary>Chi tiết phiếu nhập kho (line item)</summary>
    public class InventoryReceiptDetail
    {
        public int Id { get; set; }

        public int ReceiptId { get; set; }

        public int ProductId { get; set; }

        // Số lượng nhập
        public int Quantity { get; set; }

        // Đơn giá nhập (có thể khác UnitPrice trên Product)
        public decimal UnitCost { get; set; }

        // Navigation
        public InventoryReceipt Receipt { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
