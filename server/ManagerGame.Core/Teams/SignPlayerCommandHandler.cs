namespace ManagerGame.Core.Teams;

public class SignPlayerCommandHandler(ITeamSigningService signingService)
    : ICommandHandler<SignPlayerRequest, Team>
{
    public async Task<Result<Team>> Handle(SignPlayerRequest command,
        CancellationToken cancellationToken = default)
    {
        return await signingService.SignPlayer(command.TeamId, command.PlayerId, cancellationToken);
    }
}
