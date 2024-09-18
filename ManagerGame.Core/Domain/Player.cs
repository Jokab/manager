namespace ManagerGame.Core.Domain;

public class Player : Entity
{
    public Player(Guid id,
        Guid? teamId,
        PlayerName name,
        Position position,
        MarketValue marketValue,
        CountryRec country)
        : base(id)
    {
        TeamId = teamId;
        Name = name;
        Position = position;
        MarketValue = marketValue;
        Country = country;
    }

    public Guid? TeamId { get; init; }
    public PlayerName Name { get; init; }
    public Position Position { get; init; }
    public MarketValue MarketValue { get; init; }
    public CountryRec Country { get; init; }

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


public record MarketValue
{
    public decimal Value { get; }
	
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
    public Country Country { get; }

    public CountryRec(Country country)
    {
        Country = country;
    }

    public override string ToString()
    {
        return Country switch
        {
            Country.Se => "Sverige",
            _ => throw new ArgumentOutOfRangeException(nameof(Country), "Unsupported country")
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
