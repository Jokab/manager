using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Leagues;

public class AdmitTeamRequest : ICommand<League>
{
    public Guid LeagueId { get; set; }
    public Guid TeamId { get; set; }
}
