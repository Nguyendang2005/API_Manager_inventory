using SmartWarehouse.Application.DTOs.Supplier;

namespace SmartWarehouse.Application.Contracts.Services
{
    /// <summary>Business contract cho Supplier CRUD + validation</summary>
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllAsync(CancellationToken ct = default);
        Task<List<SupplierDto>> SearchByNameAsync(string name, CancellationToken ct = default);
        Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateSupplierDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
