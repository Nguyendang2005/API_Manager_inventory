using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Application.Contracts.Persistence
{
    /// <summary>Contract truy vấn chuyên biệt cho Supplier</summary>
    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetAllAsync(CancellationToken ct = default);
        Task<List<Supplier>> SearchByNameAsync(string name, CancellationToken ct = default);
        Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken ct = default);
        Task<bool> TaxCodeExistsAsync(string taxCode, int? excludeId = null, CancellationToken ct = default);
        Task AddAsync(Supplier supplier, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
