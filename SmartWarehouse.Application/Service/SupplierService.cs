using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.Supplier;
using SmartWarehouse.Application.Exceptions;
using SmartWarehouse.Domain.Entities;
using System.Text.RegularExpressions;

namespace SmartWarehouse.Application.Service
{
    /// <summary>
    /// Business logic cho Supplier: validate code/email/taxCode, CRUD.
    /// </summary>
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repo;

        // Regex TaxCode: 10 chữ số hoặc 10-3
        private static readonly Regex _taxCodeRegex =
            new(@"^[0-9]{10}(-[0-9]{3})?$", RegexOptions.Compiled);

        public SupplierService(ISupplierRepository repo) => _repo = repo;

        public async Task<List<SupplierDto>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return list.Select(MapToDto).ToList();
        }

        public async Task<List<SupplierDto>> SearchByNameAsync(string name, CancellationToken ct = default)
        {
            var list = await _repo.SearchByNameAsync(name, ct);
            return list.Select(MapToDto).ToList();
        }

        public async Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var s = await _repo.GetByIdAsync(id, ct);
            return s is null ? null : MapToDto(s);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default)
        {
            // 1. Kiểm tra mã NCC duy nhất
            if (await _repo.CodeExistsAsync(dto.SupplierCode, ct: ct))
                throw new DuplicateWarehouseCodeException(dto.SupplierCode);

            // 2. Validate taxCode
            if (!string.IsNullOrWhiteSpace(dto.TaxCode) && !_taxCodeRegex.IsMatch(dto.TaxCode))
                throw new ArgumentException("TaxCode không hợp lệ.");

            var supplier = new Supplier
            {
                SupplierCode = dto.SupplierCode.Trim().ToUpperInvariant(),
                Name         = dto.Name.Trim(),
                TaxCode      = dto.TaxCode?.Trim(),
                ContactEmail = dto.ContactEmail.Trim().ToLowerInvariant(),
                ContactPhone = dto.ContactPhone?.Trim(),
                Address      = dto.Address?.Trim(),
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            };

            await _repo.AddAsync(supplier, ct);
            await _repo.SaveChangesAsync(ct);
            return MapToDto(supplier);
        }

        public async Task<bool> UpdateAsync(int id, UpdateSupplierDto dto, CancellationToken ct = default)
        {
            var supplier = await _repo.GetByIdAsync(id, ct);
            if (supplier is null) return false;

            // Validate taxCode khi cập nhật
            if (!string.IsNullOrWhiteSpace(dto.TaxCode) && !_taxCodeRegex.IsMatch(dto.TaxCode))
                throw new ArgumentException("TaxCode không hợp lệ.");

            supplier.Name         = dto.Name.Trim();
            supplier.TaxCode      = dto.TaxCode?.Trim();
            supplier.ContactEmail = dto.ContactEmail.Trim().ToLowerInvariant();
            supplier.ContactPhone = dto.ContactPhone?.Trim();
            supplier.Address      = dto.Address?.Trim();
            supplier.IsActive     = dto.IsActive;

            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var supplier = await _repo.GetByIdAsync(id, ct);
            if (supplier is null) return false;

            // Soft delete — không xoá vật lý
            supplier.IsActive = false;
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        private static SupplierDto MapToDto(Supplier s) => new()
        {
            Id           = s.Id,
            SupplierCode = s.SupplierCode,
            Name         = s.Name,
            TaxCode      = s.TaxCode,
            ContactEmail = s.ContactEmail,
            ContactPhone = s.ContactPhone,
            Address      = s.Address,
            IsActive     = s.IsActive,
            CreatedAt    = s.CreatedAt
        };
    }
}
