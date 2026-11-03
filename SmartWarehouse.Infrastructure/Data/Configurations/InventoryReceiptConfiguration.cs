using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations
{
    public class InventoryReceiptConfiguration : IEntityTypeConfiguration<InventoryReceipt>
    {
        public void Configure(EntityTypeBuilder<InventoryReceipt> builder)
        {
            builder.ToTable("InventoryReceipts");
            builder.HasKey(r => r.Id);
            builder.HasIndex(r => r.ReceiptCode).IsUnique();
            builder.Property(r => r.ReceiptCode).IsRequired().HasMaxLength(50);
            builder.Property(r => r.Notes).HasMaxLength(500);

            // Warehouse → Receipt: Restrict (không xoá kho khi còn phiếu nhập)
            builder.HasOne(r => r.Warehouse)
                .WithMany(w => w.InventoryReceipts)
                .HasForeignKey(r => r.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Supplier → Receipt: Restrict (không xoá NCC khi còn phiếu nhập)
            builder.HasOne(r => r.Supplier)
                .WithMany(s => s.InventoryReceipts)
                .HasForeignKey(r => r.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N: Receipt → Details (cascade delete detail khi xoá phiếu)
            builder.HasMany(r => r.Details)
                .WithOne(d => d.Receipt)
                .HasForeignKey(d => d.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class InventoryReceiptDetailConfiguration : IEntityTypeConfiguration<InventoryReceiptDetail>
    {
        public void Configure(EntityTypeBuilder<InventoryReceiptDetail> builder)
        {
            builder.ToTable("InventoryReceiptDetails");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.UnitCost).HasPrecision(18, 2);
        }
    }
}
