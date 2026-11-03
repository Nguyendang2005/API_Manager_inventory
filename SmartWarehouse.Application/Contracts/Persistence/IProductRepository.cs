using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Application.Contracts.Persistence
{
    /// <summary>Contract truy vấn chuyên biệt cho Product</summary>
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(CancellationToken ct = default);
        Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default);
        Task<bool> SkuExistsAsync(string sku, int? excludeId = null, CancellationToken ct = default);
        Task AddAsync(Product product, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
