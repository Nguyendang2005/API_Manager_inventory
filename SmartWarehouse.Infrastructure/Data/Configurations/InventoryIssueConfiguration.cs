using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations
{
    public class InventoryIssueConfiguration : IEntityTypeConfiguration<InventoryIssue>
    {
        public void Configure(EntityTypeBuilder<InventoryIssue> builder)
        {
            builder.ToTable("InventoryIssues");
            builder.HasKey(i => i.Id);
            builder.HasIndex(i => i.IssueCode).IsUnique();
            builder.Property(i => i.IssueCode).IsRequired().HasMaxLength(50);
            builder.Property(i => i.Notes).HasMaxLength(500);

            // Warehouse → Issue: Restrict
            builder.HasOne(i => i.Warehouse)
                .WithMany(w => w.InventoryIssues)
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N: Issue → IssueDetails
            builder.HasMany(i => i.Details)
                .WithOne(d => d.Issue)
                .HasForeignKey(d => d.IssueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class InventoryIssueDetailConfiguration : IEntityTypeConfiguration<InventoryIssueDetail>
    {
        public void Configure(EntityTypeBuilder<InventoryIssueDetail> builder)
        {
            builder.ToTable("InventoryIssueDetails");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.UnitPrice).HasPrecision(18, 2);
        }
    }
}
