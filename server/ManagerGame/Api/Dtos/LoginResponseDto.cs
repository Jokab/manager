namespace ManagerGame.Api.Dtos;

public record LoginResponseDto
{
    public required ManagerDto Manager { get; set; }
    public required string Token { get; set; }
}
