using System.Text.Json.Serialization;

namespace ManagerGame.Core.Domain;

public class Team : Entity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	[JsonConstructor]
	private Team() : base(Guid.NewGuid())
	{}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	private Team(Guid id,
        TeamName name,
        Guid managerId,
        ICollection<Player> players)
        : base(id)
    {
        Name = name;
        ManagerId = managerId;
        Players = players;
    }

    public TeamName Name { get; set; }
    public Guid ManagerId { get; set; }
    public ICollection<Player> Players { get; set; } = [];

    public static Team Create(TeamName name,
        Guid managerId,
        ICollection<Player> players)
    {
        return new Team(Guid.NewGuid(), name, managerId, players);
    }

    public void SignPlayer(Player player)
    {
	    if (Players.Contains(player))
	    {
		    throw new ArgumentException($"Player with ID {player.Id} already added");
	    }
	    Players.Add(player);
    }
}
