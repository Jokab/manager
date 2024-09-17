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

public record MarketValue
{
	public decimal Value { get; private init; }
	
	public MarketValue(decimal value)
	{
		if (value < 0)
		{
			throw new ArgumentException("Value cannot be < 0");
		}
		Value = value;
	}

	public override string ToString()
	{
		return Value.ToString("C");
	}
}


public enum Country
{
	Se
}

public record CountryRec
{
	private readonly Country _country;

	public CountryRec(Country country)
	{
		_country = country;
	}

	public override string ToString()
	{
		return _country switch
		{
			Country.Se => "Sverige",
			_ => throw new ArgumentOutOfRangeException(nameof(_country), "Unsupported country")
		};
	}
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
