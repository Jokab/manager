namespace ManagerGame.Core.Drafting;

public class CreateDraftHandler(ApplicationDbContext dbContext)
    : ICommandHandler<CreateDraftRequest, Draft>
{
    public async Task<Result<Draft>> Handle(CreateDraftRequest command,
        CancellationToken cancellationToken = default)
    {
        var league = await dbContext.LeaguesRepo.Find(command.LeagueId, cancellationToken);
        if (league is null) return Result<Draft>.Failure(Error.NotFound);

        var participantTeamIds = league.Teams.Select(x => x.Id).ToList();
        var draft = Draft.DoubledPeakTraversalDraft(league.Id, participantTeamIds);

        dbContext.DraftsRepo.Add(draft);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Draft>.Success(draft);
    }
}
