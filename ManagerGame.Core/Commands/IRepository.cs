using System.Linq.Expressions;
using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public interface IRepository<T> where T : Entity
{
	Task<T> Add(T entity, CancellationToken cancellationToken = default);

	// Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
	Task<T?> Find(Guid id, CancellationToken cancellationToken = default);
}
