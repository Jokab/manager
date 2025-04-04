using System.Text.Json.Serialization;

namespace ManagerGame.Domain;

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
        ICollection<TeamPlayer> players,
        League? league)
        : base(id)
    {
        Name = name;
        ManagerId = managerId;
        Players = players;
        League = league;
    }

    public TeamName Name { get; init; }
    public Guid ManagerId { get; init; }
    public virtual ICollection<TeamPlayer> Players { get; init; } = [];
    public Guid? LeagueId { get; }
    public League? League { get; private init; }

    public static Team Create(TeamName name,
        Guid managerId,
        ICollection<Player> players,
        League? league)
    {
        var team = new Team(Guid.NewGuid(), name, managerId, [], league);
        foreach (Player? player in players) team.SignPlayer(player);

        return team;
    }

    public void SignPlayer(Player newPlayer)
    {
        if (newPlayer.IsSigned) throw new ArgumentException("Player is already signed");
        if (Players.Any(x => x.Id == newPlayer.Id)) throw new ArgumentException($"Player with ID {newPlayer.Id} already added");
        if (Players.Count(x => x.Player.Country == newPlayer.Country) >= PlayersFromSameCountryLimit)
            throw new ArgumentException($"Cannot have more players than {PlayersFromSameCountryLimit} of same country");
        if (Players.Count >= PlayerLimit)
            throw new ArgumentException($"Cannot have more than {PlayerLimit} players");
        if (SigningWillProhibitValidFormation(newPlayer))
            throw new ArgumentException(
                $"Signing {newPlayer.Position.ToString()} will make it impossible to form valid formation");

        Players.Add(new TeamPlayer(Guid.NewGuid(), this, newPlayer));
        newPlayer.TeamId = Id;
    }

    private bool SigningWillProhibitValidFormation(Player player)
    {
        foreach (Formation? formation in Formation.ValidFormations)
        {
            var requiredForPosition = formation.Positions[player.Position] -
                                      Players.Count(x => x.Player.Position == player.Position);
            var signingsRemaining = PlayerLimit - Players.Count;
            if (requiredForPosition > signingsRemaining) return true;
        }

        return false;
    }
}
