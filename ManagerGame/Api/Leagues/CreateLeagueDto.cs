using System.Text.Json.Serialization;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Leagues;

public class CreateLeagueDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public CreateLeagueDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    public CreateLeagueDto(League league)
    {
        Id = league.Id;
    }
    public Guid Id { get; set; }
}