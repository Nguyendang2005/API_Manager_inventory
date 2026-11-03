namespace SmartWarehouse.Application.Contracts.Persistence
{
    /// <summary>
    /// Unit of Work đảm bảo tất cả repository dùng chung 1 DbContext.
    /// CommitAsync() gọi SaveChangesAsync() một lần duy nhất — đảm bảo tính nguyên tử.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> CommitAsync(CancellationToken ct = default);
    }
}
