using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Infrastructure.Data;

namespace SmartWarehouse.Infrastructure.Repositories
{
    /// <summary>Truy vấn chuyên biệt cho Supplier — search by name, check code uniqueness</summary>
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext _context;

        public SupplierRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Supplier>> GetAllAsync(CancellationToken ct = default)
            => await _context.Suppliers.AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync(ct);

        public async Task<List<Supplier>> SearchByNameAsync(string name, CancellationToken ct = default)
            => await _context.Suppliers.AsNoTracking()
                .Where(s => s.IsActive && s.Name.Contains(name))
                .OrderBy(s => s.Name)
                .ToListAsync(ct);

        public async Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task<bool> CodeExistsAsync(
            string code, int? excludeId = null, CancellationToken cancellationToken = default)
            => await _context.Suppliers.AsNoTracking()
                .AnyAsync(s => s.SupplierCode == code
                            && (!excludeId.HasValue || s.Id != excludeId.Value), cancellationToken);

        public async Task<bool> TaxCodeExistsAsync(
            string taxCode, int? excludeId = null, CancellationToken ct = default)
            => await _context.Suppliers.AsNoTracking()
                .AnyAsync(s => s.TaxCode == taxCode
                            && (!excludeId.HasValue || s.Id != excludeId.Value), ct);

        public async Task AddAsync(Supplier supplier, CancellationToken ct = default)
            => await _context.Suppliers.AddAsync(supplier, ct);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}
