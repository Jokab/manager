namespace ManagerGame.Core.Commands;

public class LoginCommandHandler(ApplicationDbContext dbContext) : ICommandHandler<LoginRequest, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginRequest request,
        CancellationToken cancellationToken)
    {
        var manager = await dbContext.Managers.FindAsync([request.ManagerId], cancellationToken);
        if (manager == null) return Result<LoginResponse>.Failure(Error.NotFound);

        const string token = "token";

        return Result<LoginResponse>.Success(new LoginResponse(manager, token));
    }
}