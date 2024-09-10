using ManagerGame.Api;
using ManagerGame.Core;
using ManagerGame.Core.Commands;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        // ValidAudience = configuration["JWT:ValidAudience"],
        // ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            "492394e96d224a9a658a882ffcea948e1d213f0c08c746181f14ed3a6e7743cee9e5b71e59bed2babd7fb1e2e735301bbeb63dff1bc6fc1e7f4cef199bb183b12566ef7a429b9d8968516d2ca1452ce6e3f8478e9980db37dffc0a2d784fd461d7589d2c33fded0992df093243eeace0c0088094378a6d9161f9e432fab3660a7c8955b9ea43a1cef0c409741644567a1b515cab8f3372bb3617455d726d5cc8e9b9b35e99eca3e483be99768a07c88111b108b574330a2798c03930c0166b18c751f7b4d6973bc1599a98a08770773e42183076439ed4272ea4663ce6117e65df0162f2edd77f158b4bea7c81a34b848930f3846804923abd6d0af8efdd13b0"u8
                .ToArray())
    };
});
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("user",
        policy =>
            policy
                .RequireClaim("id"));

builder.Services.AddCors();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<CreateTeamCommandHandler>();
builder.Services.AddTransient<CreateManagerCommandHandler>();
builder.Services.AddTransient<LoginCommandHandler>();


builder.Services.AddNpgsql<ApplicationDbContext>(builder.Configuration.GetConnectionString("Db"));

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapApi();
app.Run();

// Make the implicit Program class public so test projects can access it
namespace ManagerGame
{
    public abstract class Program;
}