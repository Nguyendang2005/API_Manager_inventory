using SmartWarehouse.Application.DTOs.Warehouse;
using SmartWarehouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SmartWarehouse.Application.Common;

namespace SmartWarehouse.Application.Contracts.Services
{
    public interface IWarehouseService
    {
        Task<List<WarehouseDto>> GetAllAsync();

        Task<WarehouseDto?> GetByIdAsync(int id);

        Task<WarehouseDto> CreateAsync(CreateWarehouseDto warehouse);

        Task<bool> UpdateAsync(int id, UpdateWarehouseDto warehouse);

        Task<bool> DeleteAsync(int id);
        Task<PageResult<WarehouseDto>> GetPagedAsync(WarehouseQueryParams queryParams);

    }
}
