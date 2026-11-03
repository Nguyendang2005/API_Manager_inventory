using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse.Application.DTOs.InventoryIssue
{
    public class CreateInventoryIssueDto
    {
        [Required]
        public int WarehouseId { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Today;

        public string? Notes { get; set; }

        [Required, MinLength(1)]
        public List<CreateIssueDetailDto> Details { get; set; } = new();
    }

    public class CreateIssueDetailDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
