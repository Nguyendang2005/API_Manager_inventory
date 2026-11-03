using System.Linq.Expressions;

namespace SmartWarehouse.Application.Contracts.Persistence
{
    /// <summary>
    /// Generic repository cung cấp các thao tác CRUD cơ bản cho mọi entity.
    /// Tránh viết lại code truy vấn lặp đi lặp lại.
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        // Lấy theo Id
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

        // Lấy tất cả
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

        // Lấy theo điều kiện
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        // Thêm mới
        Task AddAsync(T entity, CancellationToken ct = default);

        // Cập nhật (EF Core theo dõi entity)
        void Update(T entity);

        // Xoá
        void Remove(T entity);

        // Kiểm tra tồn tại
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    }
}
