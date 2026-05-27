

using Domain.Entities;
using Domain.Interfaces; 
using System.Collections.Concurrent;


namespace Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories;

    public UnitOfWork(InventoryDbContext context)
    {
        _context = context;
        _repositories = new ConcurrentDictionary<Type, object>();
    }

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var entityType = typeof(T);

        if (!_repositories.ContainsKey(entityType))
        {
            var repositoryInstance = new GenericRepository<T>(_context);
            _repositories.TryAdd(entityType, repositoryInstance);
        }

        return (IGenericRepository<T>)_repositories[entityType];
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}