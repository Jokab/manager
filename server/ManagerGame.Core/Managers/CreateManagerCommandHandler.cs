namespace ManagerGame.Core.Managers;

public class CreateManagerCommandHandler(IRepository<Manager> managerRepo) : ICommandHandler<CreateManagerCommand, Manager>
{
    public async Task<Result<Manager>> Handle(CreateManagerCommand command,
        CancellationToken cancellationToken = default)
    {
        var managers = await managerRepo.GetAll(cancellationToken);

        if (managers.Any(x => x.Email == command.Email)) return Result<Manager>.Failure(Error.NotFound);

        var manager = Manager.Create(command.Name, command.Email);

        var createdManager = await managerRepo.Add(manager, cancellationToken);

        return Result<Manager>.Success(createdManager);
    }
}
