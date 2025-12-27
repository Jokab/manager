using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ManagerGame.Core.Teams;

public class TeamSigningService(ApplicationDbContext dbContext) : ITeamSigningService
{
    public async Task<Result<Team>> SignPlayer(Guid teamId,
        Guid playerId,
        CancellationToken cancellationToken = default)
    {
        var team = await dbContext.Teams2.Find(teamId, cancellationToken);
        if (team is null) return Result<Team>.Failure(Error.NotFound);

        var player = await dbContext.Players2.Find(playerId, cancellationToken);
        if (player is null) return Result<Team>.Failure(Error.NotFound);

        var alreadyOwnedInLeague = await dbContext.Set<TeamPlayer>().AnyAsync(
            tp => tp.LeagueId == team.LeagueId
                  && tp.PlayerId == player.Id
                  && tp.TeamId != team.Id,
            cancellationToken);

        if (alreadyOwnedInLeague)
            return Result<Team>.Failure("Player already drafted in this league");

        try
        {
            team.SignPlayer(player);
            var teamPlayer = team.Players.Last();
            dbContext.TeamPlayers2.Add(teamPlayer);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e) when (IsUniqueRosterViolation(e))
        {
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

        var msg = exception.InnerException?.Message ?? exception.Message;
        return msg.Contains("ix_team_player_league_id_player_id", StringComparison.OrdinalIgnoreCase)
               || msg.Contains("UNIQUE constraint failed: team_player.league_id, team_player.player_id",
                   StringComparison.OrdinalIgnoreCase);
    }
}


