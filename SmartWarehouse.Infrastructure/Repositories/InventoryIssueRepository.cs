using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Infrastructure.Data;

namespace SmartWarehouse.Infrastructure.Repositories
{
    /// <summary>Repository phiếu xuất kho — phân trang + eager loading</summary>
    public class InventoryIssueRepository : IInventoryIssueRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryIssueRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<InventoryIssue>> GetPagedAsync(
            int pageIndex, int pageSize, CancellationToken ct = default)
            => await _context.InventoryIssues
                .AsNoTracking()
                .Include(i => i.Warehouse)
                .Include(i => i.Details).ThenInclude(d => d.Product)
                .OrderByDescending(i => i.IssueDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

        public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
            => await _context.InventoryIssues.CountAsync(ct);

        public async Task<InventoryIssue?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.InventoryIssues
                .Include(i => i.Warehouse)
                .Include(i => i.Details).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(i => i.Id == id, ct);

        public async Task AddAsync(InventoryIssue issue, CancellationToken ct = default)
            => await _context.InventoryIssues.AddAsync(issue, ct);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}
