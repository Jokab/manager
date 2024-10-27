using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ManagerGame.Core.Managers;

public class LoginCommandHandler(ApplicationDbContext dbContext, IConfiguration configuration)

{
    public async Task<Result<LoginResponse>> Handle(LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var manager =
            await dbContext.Managers.FirstOrDefaultAsync(x => x.Email == command.ManagerEmail, cancellationToken);
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
