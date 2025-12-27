namespace ManagerGame.Api.Requests;

public class CreateTeamRequest
{
    public string? Name { get; init; }
    public Guid? ManagerId { get; init; }
    public Guid? LeagueId { get; init; }
}
