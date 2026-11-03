using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Application.Contracts.Persistence
{
    /// <summary>Contract truy vấn cho InventoryIssue</summary>
    public interface IInventoryIssueRepository
    {
        Task<List<InventoryIssue>> GetPagedAsync(int pageIndex, int pageSize, CancellationToken ct = default);
        Task<int> GetTotalCountAsync(CancellationToken ct = default);
        Task<InventoryIssue?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(InventoryIssue issue, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
