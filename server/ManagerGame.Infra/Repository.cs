using System.Collections.ObjectModel;
using ManagerGame.Core;
using ManagerGame.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ManagerGame.Infra;

public class Repository<T> : IRepository<T> where T : Entity
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<T> _table;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _table = dbContext.Set<T>();
    }

    public async Task<T> Add(T entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        EntityEntry<T> e = _table.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return e.Entity;
    }

    public async Task<T?> Find(Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID was empty");

        T? res = await _table.FindAsync([id], cancellationToken);
        return res;
    }

    public async Task<IReadOnlyCollection<T>> GetAll(CancellationToken cancellationToken = default) =>
        new ReadOnlyCollection<T>(await _table.ToListAsync(cancellationToken: cancellationToken));

    public async Task<T> Update(T entity,
        CancellationToken cancellationToken = default)
    {
        _table.Attach(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
