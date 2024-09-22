using System.Text.Json.Serialization;

namespace ManagerGame.Core.Domain;

public class Team : Entity
{
    public const int PlayersFromSameCountryLimit = 4;
    public const int PlayerLimit = 22;
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [JsonConstructor]
    private Team(Guid id) : base(id)
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Team(Guid id,
        TeamName name,
        Guid managerId,
        ICollection<Player> players,
        League league)
        : base(id)
    {
        Name = name;
        ManagerId = managerId;
        Players = players;
        League = league;
    }

    public TeamName Name { get; init; }
    public Guid ManagerId { get; init; }
    public ICollection<Player> Players { get; init; } = [];
    public League League { get; init; }
    public Guid LeagueId { get; set; }

    public static Team Create(TeamName name,
        Guid managerId,
        ICollection<Player> players,
        League league)
    {
        var team = new Team(Guid.NewGuid(), name, managerId, [], league);
        foreach (var player in players) team.SignPlayer(player);

        return team;
    }

    public void SignPlayer(Player player)
    {
        if (Players.Contains(player)) throw new ArgumentException($"Player with ID {player.Id} already added");
        if (Players.Count(x => x.Country == player.Country) >= PlayersFromSameCountryLimit)
            throw new ArgumentException($"Cannot have more players than {PlayersFromSameCountryLimit} of same country");
        if (Players.Count >= PlayerLimit)
            throw new ArgumentException($"Cannot have more than {PlayerLimit} players");
        if (SigningWillProhibitValidFormation(player))
            throw new ArgumentException($"Signing {player.Position.ToString()} will make it impossible to form valid formation");

        Players.Add(player);
        player.TeamId = Id;
    }

    private bool SigningWillProhibitValidFormation(Player player)
    {
        foreach (var formation in Formation.ValidFormations)
        {
            var requiredForPosition = formation.Positions[player.Position] -
                                               Players.Count(x => x.Position == player.Position);
            var signingsRemaining = PlayerLimit - Players.Count;
            if (requiredForPosition > signingsRemaining)
            {
                return true;
            }
        }

        return false;
    }
}