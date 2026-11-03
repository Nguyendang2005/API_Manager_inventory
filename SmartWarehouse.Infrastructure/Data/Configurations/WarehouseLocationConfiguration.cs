using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations
{
    public class WarehouseLocationConfiguration : IEntityTypeConfiguration<WarehouseLocation>
    {
        public void Configure(EntityTypeBuilder<WarehouseLocation> builder)
        {
            builder.ToTable("WarehouseLocations");

            builder.HasKey(wl => wl.Id);

            builder.Property(wl => wl.LocationCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(wl => wl.Aisle)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(wl => wl.Shelf)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(wl => wl.Bin)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(wl => wl.Description)
                .HasMaxLength(255);

            builder.Property(wl => wl.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(wl => wl.Warehouse)
                .WithMany(w => w.Locations)
                .HasForeignKey(wl => wl.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
