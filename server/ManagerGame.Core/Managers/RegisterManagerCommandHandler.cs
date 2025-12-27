namespace ManagerGame.Core.Managers;

public class RegisterManagerCommandHandler(ApplicationDbContext dbContext)
    : ICommandHandler<RegisterManagerCommand, Manager>
{
    public async Task<Result<Manager>> Handle(RegisterManagerCommand command,
        CancellationToken cancellationToken = default)
    {
        var managers = await dbContext.Managers2.GetAll(cancellationToken);

        if (managers.Any(x => x.Email == command.Email)) return Result<Manager>.Failure(Error.NotFound);

        var manager = Manager.Create(command.Name, command.Email);

        dbContext.Managers2.Add(manager);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Manager>.Success(manager);
    }
}
