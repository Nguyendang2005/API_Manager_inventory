using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse.Application.DTOs.Supplier
{
    public class CreateSupplierDto
    {
        // Format: SUP-[A-Z0-9]{3,6}
        [Required]
        [RegularExpression(@"^SUP-[A-Z0-9]{3,6}$",
            ErrorMessage = "SupplierCode must match format SUP-XXXXX (3-6 uppercase alphanumeric).")]
        public string SupplierCode { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Mã số thuế: 10 chữ số hoặc 10-3
        [RegularExpression(@"^[0-9]{10}(-[0-9]{3})?$",
            ErrorMessage = "TaxCode must be 10 digits or 10-3 format.")]
        public string? TaxCode { get; set; }

        [Required, EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone]
        public string? ContactPhone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
    }
}
