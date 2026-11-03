using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Application.DTOs.InventoryIssue
{
    public class InventoryIssueDto
    {
        public int Id { get; set; }
        public string IssueCode { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public IssueStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<InventoryIssueDetailDto> Details { get; set; } = new();
    }

    public class InventoryIssueDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
