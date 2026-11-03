using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.DTOs.InventoryReceipt;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.Infrastructure.Repositories
{
    /// <summary>
    /// Repository phiếu nhập kho — dùng Include().ThenInclude() và LINQ projection với AsNoTracking() để tránh N+1 query.
    /// </summary>
    public class InventoryReceiptRepository : IInventoryReceiptRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryReceiptRepository(ApplicationDbContext context) => _context = context;

        /// <summary>
        /// Lấy phiếu kèm chi tiết + Product trong 1 SQL query duy nhất (Include + ThenInclude)
        /// </summary>
        public async Task<InventoryReceipt?> GetReceiptByIdAsync(int id, CancellationToken ct = default)
            => await _context.InventoryReceipts
                .Include(r => r.Warehouse)
                .Include(r => r.Supplier)
                .Include(r => r.Details)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<List<InventoryReceipt>> GetByWarehouseAsync(
            int warehouseId, CancellationToken ct = default)
            => await _context.InventoryReceipts
                .AsNoTracking()
                .Include(r => r.Supplier)
                .Include(r => r.Details).ThenInclude(d => d.Product)
                .Where(r => r.WarehouseId == warehouseId)
                .OrderByDescending(r => r.ReceivedDate)
                .ToListAsync(ct);

        /// <summary>
        /// ASM-03: Thống kê tổng hợp theo tháng dùng LINQ projection với .AsNoTracking() — thực thi đúng 1 query SQL
        /// </summary>
        public async Task<ReceiptMonthlySummaryDto?> GetMonthlyReceiptSummaryAsync(
            int warehouseId, int year, int month, CancellationToken ct = default)
        {
            var summary = await _context.InventoryReceipts
                .AsNoTracking()
                .Where(r => r.WarehouseId == warehouseId &&
                            r.ReceivedDate.Year == year &&
                            r.ReceivedDate.Month == month)
                .Select(r => new
                {
                    ReceiptId = r.Id,
                    ItemCount = r.Details.Sum(d => d.Quantity),
                    Cost = r.Details.Sum(d => d.Quantity * d.UnitCost)
                })
                .GroupBy(x => 1)
                .Select(g => new ReceiptMonthlySummaryDto
                {
                    WarehouseId = warehouseId,
                    Year = year,
                    Month = month,
                    TotalReceipts = g.Count(),
                    TotalItemsReceived = g.Sum(x => x.ItemCount),
                    TotalCost = g.Sum(x => x.Cost)
                })
                .FirstOrDefaultAsync(ct);

            return summary ?? new ReceiptMonthlySummaryDto
            {
                WarehouseId = warehouseId,
                Year = year,
                Month = month,
                TotalReceipts = 0,
                TotalItemsReceived = 0,
                TotalCost = 0
            };
        }

        public async Task AddAsync(InventoryReceipt receipt, CancellationToken ct = default)
            => await _context.InventoryReceipts.AddAsync(receipt, ct);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}
