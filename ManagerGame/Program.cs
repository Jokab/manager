using System.Text;
using ManagerGame.Api;
using ManagerGame.Api.Drafting;
using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using ManagerGame.Infra;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!))
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
builder.Services.AddTransient<SignPlayerCommandHandler>();
builder.Services.AddTransient<CreateDraftHandler>();
builder.Services.AddTransient<StartDraftHandler>();

builder.Services.AddTransient<IRepository<Player>, Repository<Player>>();
builder.Services.AddTransient<IRepository<Team>, Repository<Team>>();
builder.Services.AddTransient<IRepository<Draft>, Repository<Draft>>();

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
