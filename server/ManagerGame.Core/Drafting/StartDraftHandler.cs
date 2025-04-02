namespace ManagerGame.Core.Drafting;

public class StartDraftHandler(IRepository<Draft> draftRepo) : ICommandHandler<StartDraftRequest, Draft>
{
    public async Task<Result<Draft>> Handle(StartDraftRequest command,
        CancellationToken cancellationToken = default)
    {
        Draft? draft = await draftRepo.Find(command.DraftId, cancellationToken);
        if (draft is null) return Result<Draft>.Failure(Error.NotFound);

        draft.Start();

        Draft updatedDraft = await draftRepo.Update(draft, cancellationToken);

        return Result<Draft>.Success(updatedDraft);
    }
}
