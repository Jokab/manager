using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Leagues;

public class AdmitTeamRequest : ICommand<League>
{
    public Guid LeagueId { get; set; }
    public Guid TeamId { get; set; }
}
