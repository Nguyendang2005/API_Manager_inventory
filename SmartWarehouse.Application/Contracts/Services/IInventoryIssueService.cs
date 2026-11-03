using SmartWarehouse.Application.DTOs.InventoryIssue;
using SmartWarehouse.Application.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.Application.Contracts.Services
{
    /// <summary>Business contract cho Inventory Issue (xuất kho) — ASM-U05 & ASM-U06</summary>
    public interface IInventoryIssueService
    {
        Task<PageResult<InventoryIssueDto>> GetPagedAsync(int pageIndex, int pageSize, CancellationToken ct = default);
        Task<InventoryIssueDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<InventoryIssueDto> CreateAsync(CreateInventoryIssueDto dto, CancellationToken ct = default);
        Task<bool> ApproveAsync(int id, CancellationToken ct = default);
    }
}
