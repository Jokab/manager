namespace ManagerGame.Api.Dtos;

public class LoginResponseDto
{
    public required ManagerDto Manager { get; set; }
    public required string Token { get; set; }
}
