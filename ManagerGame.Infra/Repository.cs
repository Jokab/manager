using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Infra;

public class Repository<T>(ApplicationDbContext dbContext) : IRepository<T> where T : Entity
{
	public async Task<T> Add(T entity, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(entity, nameof(entity));

		var e = dbContext.Add(entity);
		await dbContext.SaveChangesAsync(cancellationToken);

		dbContext.Set<T>().FirstOrDefault(x => x.Id == Guid.Empty);

		return e.Entity;
	}

	public async Task<T?> Find(Guid id, CancellationToken cancellationToken = default)
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException("ID was empty");
		}

		return await dbContext.FindAsync<T>([id], cancellationToken);
	}
}
