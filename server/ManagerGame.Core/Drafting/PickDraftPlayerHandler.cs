using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ManagerGame.Core.Drafting;

public record DraftPickOutcome(Draft Draft, DraftPick Pick, Guid? NextTeamId, Team Team);

public class PickDraftPlayerHandler(ApplicationDbContext dbContext)
    : ICommandHandler<PickDraftPlayerRequest, DraftPickOutcome>
{
    public async Task<Result<DraftPickOutcome>> Handle(PickDraftPlayerRequest command,
        CancellationToken cancellationToken = default)
    {
        var draft = await dbContext.DraftsRepo.Find(command.DraftId, cancellationToken);
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

        // Sign the player to the team
        var team = await dbContext.TeamsRepo.Find(command.TeamId, cancellationToken);
        if (team is null) return Result<DraftPickOutcome>.Failure(Error.NotFound);

        var player = await dbContext.Players.FindAsync([command.PlayerId], cancellationToken);
        if (player is null) return Result<DraftPickOutcome>.Failure(Error.NotFound);

        var alreadyOwnedInLeague = await dbContext.Set<TeamPlayer>().AnyAsync(
            tp => tp.LeagueId == team.LeagueId
                  && tp.PlayerId == player.Id
                  && tp.TeamId != team.Id,
            cancellationToken);

        if (alreadyOwnedInLeague)
            return Result<DraftPickOutcome>.Failure("Player already drafted in this league");

        try
        {
            team.SignPlayer(player);
        }
        catch (Exception e)
        {
            return Result<DraftPickOutcome>.Failure(e.Message);
        }

        DraftPick pick;
        try
        {
            pick = draft.RecordPick(command.TeamId, command.PlayerId);
        }
        catch (Exception ex)
        {
            return Result<DraftPickOutcome>.Failure(ex.Message);
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e) when (IsUniqueRosterViolation(e))
        {
            return Result<DraftPickOutcome>.Failure("Player already drafted in this league");
        }

        var nextTeamId = draft.PeekNextTeamId();
        return Result<DraftPickOutcome>.Success(new DraftPickOutcome(draft, pick, nextTeamId, team));
    }

    private static bool IsUniqueRosterViolation(DbUpdateException exception)
    {
        if (exception.InnerException is PostgresException { SqlState: "23505" })
            return true;

        var msg = exception.InnerException?.Message ?? exception.Message;
        return msg.Contains("ix_team_player_league_id_player_id", StringComparison.OrdinalIgnoreCase)
               || msg.Contains("UNIQUE constraint failed: team_player.league_id, team_player.player_id",
                   StringComparison.OrdinalIgnoreCase);
    }
}
