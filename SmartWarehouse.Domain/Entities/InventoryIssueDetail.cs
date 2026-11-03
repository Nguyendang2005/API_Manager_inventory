namespace SmartWarehouse.Domain.Entities
{
    /// <summary>Chi tiết phiếu xuất kho (line item)</summary>
    public class InventoryIssueDetail
    {
        public int Id { get; set; }

        public int IssueId { get; set; }

        public int ProductId { get; set; }

        // Số lượng xuất
        public int Quantity { get; set; }

        // Đơn giá xuất tại thời điểm lập phiếu
        public decimal UnitPrice { get; set; }

        // Navigation
        public InventoryIssue Issue { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
