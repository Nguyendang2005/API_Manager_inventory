using SmartWarehouse.Application.DTOs.Warehouse;
using SmartWarehouse.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.Application.Contracts.Persistence
{
    public interface IWarehouseRepository
    {
        Task<Warehouse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Warehouse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Warehouse entity, CancellationToken cancellationToken = default);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<List<Warehouse>> GetPagedAsync(WarehouseQueryParams queryParams, CancellationToken cancellationToken = default);
        Task<int> GetTotalCountAsync(string? search = null, CancellationToken cancellationToken = default);
    }
}
