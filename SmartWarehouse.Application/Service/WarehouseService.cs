using SmartWarehouse.Application.Common;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.Warehouse;
using SmartWarehouse.Application.Exceptions;
using SmartWarehouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.Application.Service
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseService(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        // Lấy danh sách tất cả kho đang hoạt động
        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
            return warehouses.Select(MapToDto).ToList();
        }

        // Lấy kho theo id
        public async Task<WarehouseDto?> GetByIdAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse is null || !warehouse.IsActive)
            {
                return null;
            }
            return MapToDto(warehouse);
        }

        // Tạo kho mới
        public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto)
        {
            ValidateWarehouse(dto.Code, dto.Name);
            string code = dto.Code.Trim().ToUpperInvariant();

            bool exists = await _warehouseRepository.CodeExistsAsync(code);
            if (exists)
            {
                throw new DuplicateWarehouseCodeException(code);
            }

            var warehouse = new Warehouse
            {
                Code = code,
                Name = dto.Name.Trim(),
                Address = dto.Address?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _warehouseRepository.AddAsync(warehouse);
            await _warehouseRepository.SaveChangesAsync();
            return MapToDto(warehouse);
        }

        // Cập nhật thông tin kho
        public async Task<bool> UpdateAsync(int id, UpdateWarehouseDto dto)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse is null)
            {
                return false;
            }

            ValidateWarehouse(dto.Code, dto.Name);
            string code = dto.Code.Trim().ToUpperInvariant();

            bool exists = await _warehouseRepository.CodeExistsAsync(code, excludeId: id);
            if (exists)
            {
                throw new DuplicateWarehouseCodeException(code);
            }

            warehouse.Code = code;
            warehouse.Name = dto.Name.Trim();
            warehouse.Address = dto.Address?.Trim();
            if (dto.IsActive.HasValue)
            {
                warehouse.IsActive = dto.IsActive.Value;
            }

            await _warehouseRepository.SaveChangesAsync();
            return true;
        }

        // Xóa kho (soft delete)
        public async Task<bool> DeleteAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse is null)
            {
                return false;
            }

            warehouse.IsActive = false;
            await _warehouseRepository.SaveChangesAsync();
            return true;
        }

        // Phân trang
        public async Task<PageResult<WarehouseDto>> GetPagedAsync(WarehouseQueryParams queryParams)
        {
            var warehouses = await _warehouseRepository.GetPagedAsync(queryParams);
            int total = await _warehouseRepository.GetTotalCountAsync(queryParams.Search);

            return new PageResult<WarehouseDto>
            {
                Items = warehouses.Select(MapToDto).ToList(),
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalCount = total
            };
        }

        private static WarehouseDto MapToDto(Warehouse warehouse)
        {
            return new WarehouseDto
            {
                Id = warehouse.Id,
                Code = warehouse.Code,
                Name = warehouse.Name,
                Address = warehouse.Address,
                IsActive = warehouse.IsActive,
                CreatedAt = warehouse.CreatedAt
            };
        }

        private static void ValidateWarehouse(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Warehouse code is required.");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Warehouse name is required.");
            }
        }
    }
}
