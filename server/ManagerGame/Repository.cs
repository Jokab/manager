using System.Collections.ObjectModel;
using ManagerGame.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ManagerGame;

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
        ArgumentNullException.ThrowIfNull(entity);

        // Default Add so EF will correctly discover and persist related graphs (owned types, etc).
        // Special-case TeamPlayer since it may already be tracked with an incorrect state when added
        // via a navigation collection (causing UPDATE instead of INSERT on relational providers).
        T e;
        if (entity is TeamPlayer)
        {
            var entry = _dbContext.Entry(entity);
            entry.State = EntityState.Added;
            e = entity;
        }
        else
        {
            e = _table.Add(entity).Entity;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);

        return e;
    }

    public async Task<T?> Find(Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID was empty");

        // Some aggregates rely on collection navigation updates (e.g. Team.SignPlayer adds a TeamPlayer).
        // If we load the aggregate via FindAsync without loading the collection, EF may not track the
        // newly-added join entity correctly under relational providers (SQLite/Postgres), leading to
        // UPDATEs against non-existent rows (optimistic concurrency failures) instead of INSERTs.
        if (typeof(T) == typeof(Team))
        {
            var team = await _dbContext.Set<Team>()
                .Include(x => x.Players)
                .ThenInclude(x => x.Player)
                .Include(x => x.League)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return (T?)(object?)team;
        }

        var res = await _table.FindAsync([id], cancellationToken);
        return res;
    }

    public async Task<IReadOnlyCollection<T>> GetAll(CancellationToken cancellationToken = default) =>
        new ReadOnlyCollection<T>(await _table.ToListAsync(cancellationToken));

    public async Task<T> Update(T entity,
        CancellationToken cancellationToken = default)
    {
        // IMPORTANT:
        // Most command handlers load the aggregate via this repository (tracked by the DbContext),
        // then mutate it and call Update(). In that common flow we should NOT call DbSet.Update(entity),
        // because it forces the entire graph to EntityState.Modified, turning new child entities
        // (e.g. newly-added TeamPlayer) into "Modified" -> EF issues UPDATE instead of INSERT.
        var entry = _dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _table.Attach(entity);
            entry.State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
