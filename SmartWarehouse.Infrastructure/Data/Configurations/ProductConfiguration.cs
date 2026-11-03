using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.SKU).IsUnique();
            builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Unit).HasMaxLength(20).HasDefaultValue("Piece");
            builder.Property(p => p.UnitWeightKg).HasPrecision(10, 3);
            builder.Property(p => p.UnitPrice).HasPrecision(18, 2);
        }
    }
}
