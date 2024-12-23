namespace ManagerGame.Core.Drafting;

public class CreateDraftHandler(IRepository<Draft> draftRepo, IRepository<League> leagueRepo) : ICommandHandler<CreateDraftRequest, Draft>
{
    public async Task<Result<Draft>> Handle(CreateDraftRequest command,
        CancellationToken cancellationToken = default)
    {
        var league = await leagueRepo.Find(command.LeagueId, cancellationToken);
        if (league is null) return Result<Draft>.Failure(Error.NotFound);

        var draft = Draft.DoubledPeakTraversalDraft(league);

        var newDraft = await draftRepo.Add(draft, cancellationToken);

        return Result<Draft>.Success(newDraft);
    }
}
