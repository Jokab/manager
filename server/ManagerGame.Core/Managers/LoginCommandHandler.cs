using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ManagerGame.Core.Managers;

public class LoginCommandHandler(IRepository<Manager> managerRepo, IConfiguration configuration) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var managers = await managerRepo.GetAll(cancellationToken);
        var manager = managers.FirstOrDefault(x => x.Email == command.ManagerEmail);
        if (manager == null) return Result<LoginResponse>.Failure(Error.NotFound);

        var token = GenerateToken(configuration);

        return Result<LoginResponse>.Success(new LoginResponse(manager, token));
    }

    private static string GenerateToken(IConfiguration configuration)
    {
        var jwtSecret = configuration["JWT:Secret"];
        ArgumentException.ThrowIfNullOrEmpty(jwtSecret, "Could not extract JWT secret from config");

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", "hej") }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
