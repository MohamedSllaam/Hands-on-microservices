using Domain.Common.Specifications;
using Domain.Entities; 

namespace Domain.Interfaces; 
public interface IGenericRepository<T> where T : BaseEntity
{
   
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<T?> GetEntityWithSpecAsync(ISpecifications<T> spec, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindWithSpecificationAsync(ISpecifications<T> spec, CancellationToken cancellationToken = default);
    Task<T?> FindOneWithSpecificationAsync(ISpecifications<T> spec, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecifications<T> spec, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecifications<T> spec, CancellationToken cancellationToken = default);

    // Command methods (write)
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);

    // Utility methods
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
