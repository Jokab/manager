using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Commands;

public class CreateManagerCommandHandler(ApplicationDbContext dbContext) : ICommandHandler<CreateManagerRequest, Manager>
{
    public async Task<Result<Manager>> Handle(CreateManagerRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.Managers.AnyAsync(x => x.Email == request.Email, cancellationToken);
        if (exists)
        {
            return Result<Manager>.Failure(Error.NotFound);
        }

        var manager = new Manager(request.Name, request.Email);

        var createdManager = dbContext.Managers.Add(manager);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Manager>.Success(createdManager.Entity);
    }
}
