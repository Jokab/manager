namespace ManagerGame.Core.Drafting;

public class StartDraftHandler(ApplicationDbContext dbContext) : ICommandHandler<StartDraftRequest, Draft>
{
    public async Task<Result<Draft>> Handle(StartDraftRequest command,
        CancellationToken cancellationToken = default)
    {
        var draft = await dbContext.Drafts2.Find(command.DraftId, cancellationToken);
        if (draft is null) return Result<Draft>.Failure(Error.NotFound);

        var picksPerTeam = command.PicksPerTeam.GetValueOrDefault(Team.PlayerLimit);
        draft.Start(picksPerTeam);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Draft>.Success(draft);
    }
}
