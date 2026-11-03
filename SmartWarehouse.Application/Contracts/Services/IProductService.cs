using SmartWarehouse.Application.DTOs.Product;

namespace SmartWarehouse.Application.Contracts.Services
{
    /// <summary>Business contract cho Product CRUD</summary>
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default);
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
