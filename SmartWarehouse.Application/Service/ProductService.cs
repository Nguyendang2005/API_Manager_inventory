using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.Product;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Exceptions;

namespace SmartWarehouse.Application.Service
{
    /// <summary>Business logic cho Product: validate SKU duy nhất, CRUD</summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo) => _repo = repo;

        public async Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return list.Select(MapToDto).ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var p = await _repo.GetByIdAsync(id, ct);
            return p is null ? null : MapToDto(p);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
        {
            // Kiểm tra SKU duy nhất — ném DuplicateSkuException nếu trùng
            if (await _repo.SkuExistsAsync(dto.SKU, null, ct))
                throw new DuplicateSkuException(dto.SKU);

            var product = new Product
            {
                SKU           = dto.SKU.Trim().ToUpperInvariant(),
                Name          = dto.Name.Trim(),
                Description   = dto.Description?.Trim(),
                Unit          = dto.Unit,
                UnitWeightKg  = dto.UnitWeightKg,
                UnitPrice     = dto.UnitPrice,
                CategoryId    = dto.CategoryId,
                StockQuantity = 0,
                IsActive      = true,
                CreatedAt     = DateTime.UtcNow
            };

            await _repo.AddAsync(product, ct);
            await _repo.SaveChangesAsync(ct);
            return MapToDto(product);
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
        {
            var product = await _repo.GetByIdAsync(id, ct);
            if (product is null) return false;

            product.Name         = dto.Name.Trim();
            product.Description  = dto.Description?.Trim();
            product.Unit         = dto.Unit;
            product.UnitWeightKg = dto.UnitWeightKg;
            product.UnitPrice    = dto.UnitPrice;
            product.IsActive     = dto.IsActive;

            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var product = await _repo.GetByIdAsync(id, ct);
            if (product is null) return false;

            product.IsActive = false; // soft delete
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        private static ProductDto MapToDto(Product p) => new()
        {
            Id            = p.Id,
            SKU           = p.SKU,
            Name          = p.Name,
            Description   = p.Description,
            Unit          = p.Unit,
            UnitWeightKg  = p.UnitWeightKg,
            UnitPrice     = p.UnitPrice,
            StockQuantity = p.StockQuantity,
            IsActive      = p.IsActive,
            CreatedAt     = p.CreatedAt
        };
    }
}
