using SmartWarehouse.Application.DTOs.InventoryReceipt;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.Application.Contracts.Services
{
    /// <summary>Business contract cho Inventory Receipt — ASM-U04</summary>
    public interface IInventoryReceiptService
    {
        Task<InventoryReceiptDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<InventoryReceiptDto>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default);
        Task<ReceiptMonthlySummaryDto?> GetMonthlyReceiptSummaryAsync(int warehouseId, int year, int month, CancellationToken ct = default);
        Task<InventoryReceiptDto> CreateAsync(CreateInventoryReceiptDto dto, CancellationToken ct = default);
        Task<bool> ConfirmAsync(int id, CancellationToken ct = default);
    }
}
