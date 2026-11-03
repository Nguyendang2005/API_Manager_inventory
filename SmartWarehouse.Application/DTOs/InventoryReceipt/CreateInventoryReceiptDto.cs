
using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse.Application.DTOs.InventoryReceipt
{
    public class CreateInventoryReceiptDto
    {
        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        public DateTime ReceivedDate { get; set; } = DateTime.Today;

        public string? Notes { get; set; }

        [Required, MinLength(1)]
        public List<CreateReceiptDetailDto> Details { get; set; } = new();
    }

    public class CreateReceiptDetailDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }
    }
}
