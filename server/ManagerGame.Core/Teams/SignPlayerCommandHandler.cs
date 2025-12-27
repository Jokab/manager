using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ManagerGame.Core.Teams;

public class SignPlayerCommandHandler(
    ApplicationDbContext dbContext)
    : ICommandHandler<SignPlayerRequest, Team>
{
    public async Task<Result<Team>> Handle(SignPlayerRequest command,
        CancellationToken cancellationToken = default)
    {
        var team = await dbContext.Teams2.Find(command.TeamId, cancellationToken);
        if (team is null) return Result<Team>.Failure(Error.NotFound);

        var player = await dbContext.Players2.Find(command.PlayerId, cancellationToken);
        if (player is null) return Result<Team>.Failure(Error.NotFound);

        var leagueId = team.LeagueId;
        var alreadyOwnedInLeague = await dbContext.Set<TeamPlayer>().AnyAsync(
            tp => tp.LeagueId == leagueId
                  && tp.PlayerId == player.Id
                  && tp.TeamId != team.Id,
            cancellationToken);

        if (alreadyOwnedInLeague)
            return Result<Team>.Failure("Player already drafted in this league");

        try
        {
            team.SignPlayer(player);
            // Persist the join entity explicitly.
            // Relying on aggregate graph updates has proven brittle across providers (SQLite/Postgres),
            // and can lead to UPDATEs against non-existent rows.
            var teamPlayer = team.Players.Last();
            dbContext.TeamPlayers2.Add(teamPlayer);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e) when (IsUniqueRosterViolation(e))
        {
            // Concurrency-safe enforcement: if two teams try to sign the same player in the same league
            // at the same time, the DB unique constraint is the final arbiter.
            return Result<Team>.Failure("Player already drafted in this league");
        }
        catch (Exception e)
        {
            return Result<Team>.Failure(e.Message);
        }

        return Result<Team>.Success(team);
    }

    private static bool IsUniqueRosterViolation(DbUpdateException exception)
    {
        if (exception.InnerException is PostgresException { SqlState: "23505" })
            return true;

        // SQLite falls back to message parsing
        var msg = exception.InnerException?.Message ?? exception.Message;
        return msg.Contains("ix_team_player_league_id_player_id", StringComparison.OrdinalIgnoreCase)
               || msg.Contains("UNIQUE constraint failed: team_player.league_id, team_player.player_id",
                   StringComparison.OrdinalIgnoreCase);
    }
}
