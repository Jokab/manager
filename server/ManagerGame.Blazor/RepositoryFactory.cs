using Microsoft.EntityFrameworkCore;
using ManagerGame.Core;
using ManagerGame.Domain;

// Create a factory class for the repository since we don't have direct access to the concrete Repository class
namespace ManagerGame.Blazor;

public class RepositoryFactory
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryFactory(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IRepository<T> CreateRepository<T>() where T : Entity
    {
        return new EntityRepository<T>(_dbContext);
    }

    private class EntityRepository<T> : IRepository<T> where T : Entity
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _table;

        public EntityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _table = dbContext.Set<T>();
        }

        public async Task<T> Add(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var e = _table.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return e.Entity;
        }

        public async Task<T?> Find(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) throw new ArgumentException("ID was empty");

            return await _table.FindAsync([id], cancellationToken);
        }

        public async Task<IReadOnlyCollection<T>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _table.ToListAsync(cancellationToken);
        }

        public async Task<T> Update(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}
