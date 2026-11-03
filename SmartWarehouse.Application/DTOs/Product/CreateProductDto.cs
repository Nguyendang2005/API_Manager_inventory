using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse.Application.DTOs.Product
{
    public class CreateProductDto
    {
        [Required, MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; } = "Piece";

        [Range(0, 99999)]
        public decimal UnitWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public int CategoryId { get; set; }
    }
}
