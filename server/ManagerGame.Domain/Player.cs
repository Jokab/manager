namespace ManagerGame.Domain;

public class Player : Entity
{
    public Player(
        PlayerName name,
        Position position,
        CountryRec country)
    {
        Name = name;
        Position = position;
        Country = country;
        Eliminated = false;
    }

    public PlayerName Name { get; init; }
    public Position Position { get; init; }
    public CountryRec Country { get; init; }

    // Added for match tracking
    public bool Eliminated { get; set; }

    // References to team player associations
    public virtual ICollection<TeamPlayer> TeamPlayers { get; private set; } = [];
    public virtual ICollection<StartingElevenPlayer> StartingElevenPlayers { get; private set; } = [];

    public static IEqualityComparer<Player> IdNamePositionComparer { get; } = new IdNamePositionEqualityComparer();

    private sealed class IdNamePositionEqualityComparer : IEqualityComparer<Player>
    {
        // ReSharper disable once CyclomaticComplexity it's fine
        public bool Equals(Player? x,
            Player? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id) && x.Name.Equals(y.Name) && x.Position == y.Position;
        }

        public int GetHashCode(Player obj) => HashCode.Combine(obj.Id, obj.Name, (int)obj.Position);
    }
}

public enum Country
{
    Se,
    Dk,
    En,
    De,
    No,
    Es
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
    Forward,
    Goalkeeper
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
