using System;

namespace SmartWarehouse.Domain.Entities
{
    /// <summary>
    /// Bin/Aisle/Shelf storage location inside a warehouse
    /// </summary>
    public class WarehouseLocation
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }

        public string LocationCode { get; set; } = string.Empty;

        public string Aisle { get; set; } = string.Empty;

        public string Shelf { get; set; } = string.Empty;

        public string Bin { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Warehouse Warehouse { get; set; } = null!;
    }
}
