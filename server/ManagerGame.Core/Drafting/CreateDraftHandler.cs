namespace ManagerGame.Core.Drafting;

public class CreateDraftHandler(ApplicationDbContext dbContext)
    : ICommandHandler<CreateDraftRequest, Draft>
{
    public async Task<Result<Draft>> Handle(CreateDraftRequest command,
        CancellationToken cancellationToken = default)
    {
        var league = await dbContext.Leagues2.Find(command.LeagueId, cancellationToken);
        if (league is null) return Result<Draft>.Failure(Error.NotFound);

        var draft = Draft.DoubledPeakTraversalDraft(league);

        dbContext.Drafts2.Add(draft);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Draft>.Success(draft);
    }
}
