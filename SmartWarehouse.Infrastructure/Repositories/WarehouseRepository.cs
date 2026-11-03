using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.DTOs.Warehouse;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.Infrastructure.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ApplicationDbContext _context;

        public WarehouseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Warehouse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Warehouses
                .AsNoTracking()
                .Where(w => w.IsActive)
                .OrderBy(w => w.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<Warehouse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await _context.Warehouses
                .AsNoTracking()
                .AnyAsync(w => w.Code == code && (!excludeId.HasValue || w.Id != excludeId.Value), cancellationToken);
        }

        public async Task AddAsync(Warehouse entity, CancellationToken cancellationToken = default)
        {
            await _context.Warehouses.AddAsync(entity, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Warehouse>> GetPagedAsync(WarehouseQueryParams queryParams, CancellationToken cancellationToken = default)
        {
            var query = _context.Warehouses.AsNoTracking().Where(w => w.IsActive);

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var s = queryParams.Search.Trim().ToLower();
                query = query.Where(w => w.Code.ToLower().Contains(s) || w.Name.ToLower().Contains(s));
            }

            int skip = (queryParams.PageNumber - 1) * queryParams.PageSize;
            return await query
                .OrderBy(w => w.Id)
                .Skip(skip)
                .Take(queryParams.PageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(string? search = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Warehouses.AsNoTracking().Where(w => w.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(w => w.Code.ToLower().Contains(s) || w.Name.ToLower().Contains(s));
            }

            return await query.CountAsync(cancellationToken);
        }
    }
}
