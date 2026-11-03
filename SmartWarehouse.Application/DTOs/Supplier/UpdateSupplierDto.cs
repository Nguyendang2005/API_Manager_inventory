using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse.Application.DTOs.Supplier
{
    public class UpdateSupplierDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9]{10}(-[0-9]{3})?$",
            ErrorMessage = "TaxCode must be 10 digits or 10-3 format.")]
        public string? TaxCode { get; set; }

        [Required, EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone]
        public string? ContactPhone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
