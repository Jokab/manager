using System.Text.Json.Serialization;

namespace ManagerGame.Domain;

public sealed class Team : Entity
{
    public const int PlayersFromSameCountryLimit = 4;
    public const int PlayerLimit = 22;

    [JsonConstructor]
    private Team(Guid id) : base(id)
    {
        Players = [];
        StartingElevens = [];
    }

    private Team(Guid id,
        TeamName name,
        Guid managerId,
        ICollection<TeamPlayer> players,
        Guid leagueId)
        : base(id)
    {
        Name = name;
        ManagerId = managerId;
        Players = players;
        LeagueId = leagueId;
        StartingElevens = [];
        TotalPoints = 0;
    }

    public TeamName Name { get; init; } = null!;
    public Guid ManagerId { get; init; }
    public ICollection<TeamPlayer> Players { get; init; }
    public ICollection<StartingEleven> StartingElevens { get; init; }
    public Guid LeagueId { get; private set; }
    public League League { get; private set; } = null!;
    public int TotalPoints { get; private set; }

    public static Team Create(TeamName name,
        Guid managerId,
        ICollection<Player> players,
        Guid leagueId)
    {
        var team = new Team(Guid.NewGuid(), name, managerId, [], leagueId);
        foreach (var player in players) team.SignPlayer(player);

        return team;
    }

    public void SignPlayer(Player newPlayer)
    {
        if (Players.Any(x => x.PlayerId == newPlayer.Id)) throw new ArgumentException($"Player with ID {newPlayer.Id} already added");
        if (Players.Count(x => x.Player.Country == newPlayer.Country) >= PlayersFromSameCountryLimit)
            throw new ArgumentException($"Cannot have more players than {PlayersFromSameCountryLimit} of same country");
        if (Players.Count >= PlayerLimit)
            throw new ArgumentException($"Cannot have more than {PlayerLimit} players");
        if (SigningWillProhibitValidFormation(newPlayer))
            throw new ArgumentException(
                $"Signing {newPlayer.Position.ToString()} will make it impossible to form valid formation");

        Players.Add(new TeamPlayer(Guid.NewGuid(), this, newPlayer));
    }

    public StartingEleven CreateStartingEleven(string matchRound)
    {
        if (StartingElevens.Any(se => se.MatchRound == matchRound))
            throw new InvalidOperationException($"Team already has a starting eleven for round {matchRound}");

        var startingEleven = new StartingEleven(Guid.NewGuid(), Id, matchRound);
        StartingElevens.Add(startingEleven);
        return startingEleven;
    }

    public void AddPoints(int points)
    {
        TotalPoints += points;
    }

    private bool SigningWillProhibitValidFormation(Player player)
    {
        foreach (var formation in Formation.ValidFormations)
        {
            var requiredForPosition = formation.Positions[player.Position] -
                                      Players.Count(x => x.Player.Position == player.Position);
            var signingsRemaining = PlayerLimit - Players.Count;
            if (requiredForPosition > signingsRemaining) return true;
        }

        return false;
    }
}
