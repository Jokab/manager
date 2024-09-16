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

public class Player : Entity
{
	public Player(Guid id, Guid? teamId, PlayerName name, Position position) : base(id)
	{
		TeamId = teamId;
		Name = name;
		Position = position;
	}

	public Guid? TeamId { get; init; }
	public PlayerName Name { get; init; }
	public Position Position { get; init; }
	// public MarketValue MarketValue { get; init; }
	// public Country Country { get; init; }

	private sealed class IdNamePositionEqualityComparer : IEqualityComparer<Player>
	{
		public bool Equals(Player? x, Player? y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			if (x.GetType() != y.GetType()) return false;
			return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Position == y.Position;
		}

		public int GetHashCode(Player obj)
		{
			return HashCode.Combine(obj.Id, obj.Name, (int)obj.Position);
		}
	}

	public static IEqualityComparer<Player> IdNamePositionComparer { get; } = new IdNamePositionEqualityComparer();
}

public class MarketValue
{
}

public class Country
{
}

public enum Position
{
	Defender,
	Midfielder,
	Forward
}

public record PlayerName
{
	public PlayerName(string name)
	{
		ArgumentException.ThrowIfNullOrEmpty(name);
		Name = name;
	}

	public string Name { get; private set; }
}
