using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");
            builder.HasKey(s => s.Id);

            // Unique index trên SupplierCode
            builder.HasIndex(s => s.SupplierCode).IsUnique();

            builder.Property(s => s.SupplierCode).IsRequired().HasMaxLength(20);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.TaxCode).HasMaxLength(14);
            builder.Property(s => s.ContactEmail).HasMaxLength(150);
            builder.Property(s => s.ContactPhone).HasMaxLength(20);
            builder.Property(s => s.Address).HasMaxLength(500);
        }
    }
}
