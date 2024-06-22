using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Commands;

public class CreateManagerCommandHandler(ApplicationDbContext dbContext) : ICommandHandler<CreateManagerRequest>
{
    public async Task<Result> Handle(CreateManagerRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.Managers.AnyAsync(x => x.Email == request.Email, cancellationToken);
        if (exists)
        {
            return Result.Failure(Error.NotFound);
        }
        
        var manager = new Manager(request.Name, request.Email);

        dbContext.Managers.Add(manager);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}