using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse.Domain.Entities
{
    /// <summary>Nhà cung cấp hàng hoá cho kho</summary>
    public class Supplier
    {
        public int Id { get; set; }

        // Mã NCC duy nhất, format SUP-XXXXX
        public string SupplierCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        // Mã số thuế: 10 chữ số hoặc 10-3
        public string? TaxCode { get; set; }

        public string? ContactEmail { get; set; }

        public string? ContactPhone { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: một NCC có nhiều phiếu nhập
        public ICollection<InventoryReceipt> InventoryReceipts { get; set; }
            = new List<InventoryReceipt>();
    }
}
