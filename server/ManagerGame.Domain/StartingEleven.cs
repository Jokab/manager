namespace ManagerGame.Domain;

public class StartingEleven : Entity
{
    public StartingEleven(
        Guid teamId,
        string matchRound)
    {
        TeamId = teamId;
        MatchRound = matchRound;
        SelectedPlayers = [];
        IsLocked = false;
        PointsEarned = 0;
    }

    // For EF Core
    private StartingEleven()
    {
        SelectedPlayers = [];
    }

    public Guid TeamId { get; private init; }
    public virtual Team Team { get; private init; } = null!;

    public string MatchRound { get; private init; } = string.Empty;
    public virtual ICollection<StartingElevenPlayer> SelectedPlayers { get; private init; }
    public bool IsLocked { get; private set; }
    public int PointsEarned { get; private set; }

    public void AddPlayer(Player player)
    {
        if (IsLocked)
            throw new InvalidOperationException("Cannot modify a locked starting eleven");

        if (SelectedPlayers.Count >= 11)
            throw new InvalidOperationException("Cannot select more than 11 players");

        if (SelectedPlayers.Any(p => p.PlayerId == player.Id))
            throw new InvalidOperationException("Player already selected");

        // Check formation validity (could be more complex)
        var goalkeepers = SelectedPlayers.Count(p => p.Player.Position == Position.Goalkeeper);
        if (player.Position == Position.Goalkeeper && goalkeepers >= 1)
            throw new InvalidOperationException("Cannot select more than 1 goalkeeper");

        SelectedPlayers.Add(new StartingElevenPlayer(this, player));
    }

    public void RemovePlayer(Guid playerId)
    {
        if (IsLocked)
            throw new InvalidOperationException("Cannot modify a locked starting eleven");

        var player = SelectedPlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player == null)
            throw new InvalidOperationException("Player not in starting eleven");

        SelectedPlayers.Remove(player);
    }

    public void Lock()
    {
        if (SelectedPlayers.Count != 11)
            throw new InvalidOperationException("Must select exactly 11 players");

        // Validate formation (1 GK, at least 3 DEF, etc.)
        var goalkeepers = SelectedPlayers.Count(p => p.Player.Position == Position.Goalkeeper);
        var defenders = SelectedPlayers.Count(p => p.Player.Position == Position.Defender);
        var midfielders = SelectedPlayers.Count(p => p.Player.Position == Position.Midfielder);
        var forwards = SelectedPlayers.Count(p => p.Player.Position == Position.Forward);

        if (goalkeepers != 1)
            throw new InvalidOperationException("Must select exactly 1 goalkeeper");

        if (defenders < 3)
            throw new InvalidOperationException("Must select at least 3 defenders");

        if (forwards < 1)
            throw new InvalidOperationException("Must select at least 1 forward");

        IsLocked = true;
    }

    public void CalculatePoints(IEnumerable<MatchResult> matchResults, LeagueSettings settings)
    {
        if (!IsLocked)
            throw new InvalidOperationException("Starting eleven must be locked before calculating points");

        var points = 0;

        foreach (var matchResult in matchResults.Where(m => m.IsFinished))
        {
            foreach (var selectedPlayer in SelectedPlayers)
            {
                var player = selectedPlayer.Player;

                // Get events for this player in this match
                var playerEvents = matchResult.MatchEvents
                    .Where(e => e.PlayerId == player.Id)
                    .ToList();

                // Add points for goals
                points += playerEvents.Count(e => e.EventType == MatchEventType.Goal) * settings.PointsPerGoal;

                // Add points for assists
                points += playerEvents.Count(e => e.EventType == MatchEventType.Assist) * settings.PointsPerAssist;

                // Add points for clean sheets (goalkeepers and defenders only)
                if ((player.Position == Position.Goalkeeper || player.Position == Position.Defender) &&
                    playerEvents.Any(e => e.EventType == MatchEventType.CleanSheet))
                {
                    points += settings.PointsPerCleanSheet;
                }

                // Add points for wins
                var isHomeTeam = matchResult.MatchEvents.Any(e => e.PlayerId == player.Id && e.IsHomeTeam);
                var isPlayerTeamWinner = isHomeTeam
                    ? matchResult.HomeGoals > matchResult.AwayGoals
                    : matchResult.AwayGoals > matchResult.HomeGoals;

                if (isPlayerTeamWinner)
                    points += settings.PointsPerWin;
            }
        }

        PointsEarned = points;
    }
}
