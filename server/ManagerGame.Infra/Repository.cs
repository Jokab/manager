using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.EntityFrameworkCore;

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

        var e = _table.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return e.Entity;
    }

    public async Task<T?> Find(Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID was empty");

        var res = await _table.FindAsync([id], cancellationToken);
        return res;
    }

    public async Task<T> Update(T entity,
        CancellationToken cancellationToken = default)
    {
        _table.Attach(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
