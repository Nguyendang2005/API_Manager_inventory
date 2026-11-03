using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Infrastructure.Data;

namespace SmartWarehouse.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work: tất cả Repository chia sẻ 1 DbContext.
    /// CommitAsync() gọi SaveChangesAsync() đúng một lần — đảm bảo atomic transaction.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // Cache repository instances — không tạo lại nhiều lần
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context) => _context = context;

        /// <summary>Lấy hoặc tạo mới GenericRepository cho entity type T</summary>
        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
                _repositories[type] = new GenericRepository<T>(_context);
            return (IGenericRepository<T>)_repositories[type];
        }

        /// <summary>Commit tất cả thay đổi trong một transaction duy nhất</summary>
        public async Task<int> CommitAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

        public void Dispose() => _context.Dispose();
    }
}
