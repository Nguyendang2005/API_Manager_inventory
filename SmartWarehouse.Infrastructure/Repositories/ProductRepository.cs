using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Infrastructure.Data;

namespace SmartWarehouse.Infrastructure.Repositories
{
    /// <summary>Truy vấn chuyên biệt cho Product</summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Product>> GetAllAsync(CancellationToken ct = default)
            => await _context.Products.AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync(ct);

        public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default)
            => await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.SKU == sku, ct);

        public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null, CancellationToken ct = default)
            => await _context.Products.AsNoTracking()
                .AnyAsync(p => p.SKU == sku
                            && (!excludeId.HasValue || p.Id != excludeId.Value), ct);

        public async Task AddAsync(Product product, CancellationToken ct = default)
            => await _context.Products.AddAsync(product, ct);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}
