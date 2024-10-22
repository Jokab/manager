using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Managers;

public class CreateManagerCommandHandler(ApplicationDbContext dbContext)
    : ICommandHandler<CreateManagerCommand, Manager>
{
    public async Task<Result<Manager>> Handle(CreateManagerCommand command,
        CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.Managers.AnyAsync(x => x.Email == command.Email, cancellationToken);
        if (exists) return Result<Manager>.Failure(Error.NotFound);

        var manager = Manager.Create(command.Name, command.Email);

        var createdManager = dbContext.Managers.Add(manager);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Manager>.Success(createdManager.Entity);
    }
}
