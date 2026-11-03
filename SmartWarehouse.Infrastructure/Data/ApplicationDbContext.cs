using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data
{
    /// <summary>
    /// EF Core DbContext — đăng ký tất cả entity của hệ thống WMS theo Clean Architecture.
    /// ApplyConfigurationsFromAssembly() tự động load tất cả IEntityTypeConfiguration.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ── Warehouse domain ──
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>();

        // ── Product domain ──
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

        // ── Supplier domain ──
        public DbSet<Supplier> Suppliers => Set<Supplier>();

        // ── Inventory domain ──
        public DbSet<InventoryReceipt> InventoryReceipts => Set<InventoryReceipt>();
        public DbSet<InventoryReceiptDetail> InventoryReceiptDetails => Set<InventoryReceiptDetail>();
        public DbSet<InventoryIssue> InventoryIssues => Set<InventoryIssue>();
        public DbSet<InventoryIssueDetail> InventoryIssueDetails => Set<InventoryIssueDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
