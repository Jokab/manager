namespace ManagerGame.Core.Domain;

public class Player : Entity
{
    public Player(Guid id,
        PlayerName name,
        Position position,
        MarketValue marketValue,
        CountryRec country)
        : base(id)
    {
        Name = name;
        Position = position;
        MarketValue = marketValue;
        Country = country;
    }

    public Guid? TeamId { get; set; }
    public PlayerName Name { get; init; }
    public Position Position { get; init; }
    public MarketValue MarketValue { get; init; }
    public CountryRec Country { get; init; }

    public static IEqualityComparer<Player> IdNamePositionComparer { get; } = new IdNamePositionEqualityComparer();

    private sealed class IdNamePositionEqualityComparer : IEqualityComparer<Player>
    {
        public bool Equals(Player? x,
            Player? y)
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
}

public record MarketValue
{
    public MarketValue(decimal value)
    {
        if (value < 0) throw new ArgumentException("Value cannot be < 0");
        Value = value;
    }

    public decimal Value { get; }

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
    public CountryRec(Country country)
    {
        Country = country;
    }

    public Country Country { get; }

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
