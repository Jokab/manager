namespace ManagerGame.Core;

public interface IRepository<T> where T : Entity
{
    Task<T> Add(T entity,
        CancellationToken cancellationToken = default);

    Task<T?> Find(Guid id,
        CancellationToken cancellationToken = default);

    Task<T> Update(T entity,
        CancellationToken cancellationToken = default);
}
