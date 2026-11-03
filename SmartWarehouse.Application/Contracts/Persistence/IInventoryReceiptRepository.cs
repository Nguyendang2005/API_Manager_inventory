using SmartWarehouse.Application.DTOs.InventoryReceipt;
using SmartWarehouse.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.Application.Contracts.Persistence
{
    /// <summary>Contract truy vấn cho InventoryReceipt với eager loading & projection — ASM-U04</summary>
    public interface IInventoryReceiptRepository
    {
        // Include Details + Product để tránh N+1
        Task<InventoryReceipt?> GetReceiptByIdAsync(int id, CancellationToken ct = default);
        Task<List<InventoryReceipt>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default);
        Task<ReceiptMonthlySummaryDto?> GetMonthlyReceiptSummaryAsync(int warehouseId, int year, int month, CancellationToken ct = default);
        Task AddAsync(InventoryReceipt receipt, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
