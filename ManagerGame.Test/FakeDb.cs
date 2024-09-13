using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Test;

public class FakeRepo<T> : IRepository<T> where T : Entity
{
	private List<T> Entities { get; } = [];

	public Task<T> Add(T entity, CancellationToken cancellationToken = default)
	{
		Entities.Add(entity);
		return Task.FromResult(entity);
	}

	public Task<T?> Find(Guid id, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Entities.FirstOrDefault(x => x.Id == id));
	}
}
