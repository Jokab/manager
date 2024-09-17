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