using System.Text.Json.Serialization;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api;

public class AdmitTeamDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AdmitTeamDto(){}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        
    public AdmitTeamDto(League league)
    {
        League = league;
    }

    public League League { get; set; }
}
