namespace ManagerGame.Api.Requests;

public class CreateTeamRequest
{
    public string? Name { get; init; }
    public Guid? ManagerId { get; init; }
}