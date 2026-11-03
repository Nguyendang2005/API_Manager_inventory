using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(w => w.Id);
            builder.Property(w => w.Id)
                .ValueGeneratedOnAdd();

            builder.Property(w => w.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(w => w.Code)
                .IsUnique();

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(w => w.Address)
                .HasMaxLength(255);

            builder.Property(w => w.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(w => w.CreatedAt)
                .IsRequired();
        }
    }
}
