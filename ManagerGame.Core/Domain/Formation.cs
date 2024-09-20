namespace ManagerGame.Core.Domain;

public class Formation
{
    public readonly Dictionary<Position, int> Positions = new ();

    public Formation(int defenders,
        int midfielders,
        int forwards)
    {
        Positions[Position.Goalkeeper] = 1;
        Positions[Position.Defender] = defenders;
        Positions[Position.Midfielder] = midfielders;
        Positions[Position.Forward] = forwards;
    }

    public static readonly Formation[] ValidFormations =
    [
        new Formation(4, 4, 2)
    ];
}
