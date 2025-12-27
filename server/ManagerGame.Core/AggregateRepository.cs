using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
namespace ManagerGame.Core;

/// <param name="aggregateQuery">Should have Include calls attached to materialize the aggregate.</param>
public class AggregateRepository<T, TKey>(
    DbSet<T> dbSet,
    IQueryable<T> aggregateQuery,
    Expression<Func<T, TKey>> keySelector) where T : class
{
    public void Add(T entity) =>
        dbSet.Add(entity);

    public async Task<T?> Find(TKey id, CancellationToken cancellationToken = default) =>
        await aggregateQuery.FirstOrDefaultAsync(BuildKeyPredicate(id), cancellationToken);

    private Expression<Func<T, bool>> BuildKeyPredicate(TKey id)
    {
        var match = Expression.Equal(keySelector.Body, Expression.Constant(id));
        return Expression.Lambda<Func<T, bool>>(match, keySelector.Parameters);
    }

    public async Task<IReadOnlyCollection<T>> GetAll(CancellationToken cancellationToken = default) =>
        await aggregateQuery.ToListAsync(cancellationToken);
}
