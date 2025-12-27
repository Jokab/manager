using ManagerGame.Core.Teams;

namespace ManagerGame.Core.Drafting;

public record DraftPickOutcome(Draft Draft, DraftPick Pick, Guid? NextTeamId, Team Team);

public class PickDraftPlayerHandler(
    ApplicationDbContext dbContext,
    ITeamSigningService signingService)
    : ICommandHandler<PickDraftPlayerRequest, DraftPickOutcome>
{
    public async Task<Result<DraftPickOutcome>> Handle(PickDraftPlayerRequest command,
        CancellationToken cancellationToken = default)
    {
        var draft = await dbContext.Drafts2.Find(command.DraftId, cancellationToken);
        if (draft is null) return Result<DraftPickOutcome>.Failure(Error.NotFound);

        if (draft.State is not DraftState.Started)
            return Result<DraftPickOutcome>.Failure("Draft is not in started state");

        var isParticipant = draft.Participants.Any(x => x.TeamId == command.TeamId);
        if (!isParticipant)
            return Result<DraftPickOutcome>.Failure("Team is not part of this draft");

        var expectedTeamId = draft.PeekNextTeamId();
        if (expectedTeamId is null)
            return Result<DraftPickOutcome>.Failure("No next team available in draft");
        if (expectedTeamId.Value != command.TeamId)
            return Result<DraftPickOutcome>.Failure("It's not your turn to draft");

        var signingResult = await signingService.SignPlayer(command.TeamId, command.PlayerId, cancellationToken);
        if (signingResult.IsFailure)
            return Result<DraftPickOutcome>.Failure(signingResult.Error);

        DraftPick pick;
        try
        {
            pick = draft.RecordPick(command.TeamId, command.PlayerId);
        }
        catch (Exception ex)
        {
            return Result<DraftPickOutcome>.Failure(ex.Message);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var nextTeamId = draft.PeekNextTeamId();
        return Result<DraftPickOutcome>.Success(new DraftPickOutcome(draft, pick, nextTeamId, signingResult.Value!));
    }
}


