using System.Text;
using System.Text.Json.Serialization;
using ManagerGame;
using ManagerGame.Api;
using ManagerGame.Core;
using ManagerGame.Core.Drafting;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
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

builder.Services.AddSignalR();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

RegisterCommandHandlers();

if (builder.Environment.IsEnvironment("Test") || builder.Environment.IsEnvironment("Testing"))
{
    var connectionString =
        configuration["Test:Sqlite:ConnectionString"]
        ?? $"Data Source={Path.Combine(Path.GetTempPath(), "manager-test.db")}";
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
}
else
{
    builder.Services.AddNpgsql<ApplicationDbContext>(builder.Configuration.GetConnectionString("Db"));
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<TestHub>("/chatHub");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapApi();

if (args.Contains("seed"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    ResetDb(db);
    await SeedDb(scope, db);
}

app.Run();
return;

void RegisterCommandHandlers()
{
    AddCommandHandlerWithLogging<CreateTeamCommand, Team, CreateTeamCommandHandler>();
    AddCommandHandlerWithLogging<RegisterManagerCommand, Manager, RegisterManagerCommandHandler>();
    AddCommandHandlerWithLogging<LoginCommand, LoginResponse, LoginCommandHandler>();
    AddCommandHandlerWithLogging<SignPlayerRequest, Team, SignPlayerCommandHandler>();
    AddCommandHandlerWithLogging<CreateDraftRequest, Draft, CreateDraftHandler>();
    AddCommandHandlerWithLogging<StartDraftRequest, Draft, StartDraftHandler>();
    AddCommandHandlerWithLogging<CreateLeagueRequest, League, CreateLeagueHandler>();
    AddCommandHandlerWithLogging<AdmitTeamRequest, League, AdmitTeamHandler>();
}

void AddCommandHandlerWithLogging<TCommand, TResult, THandler>()
    where TCommand : class
    where TResult : class
    where THandler : class, ICommandHandler<TCommand, TResult>
{
    builder.Services.AddScoped<THandler>();
    builder.Services.AddScoped<ICommandHandler<TCommand, TResult>, LoggingDecorator<TCommand, TResult, THandler>>();
}

void ResetDb(ApplicationDbContext? applicationDbContext)
{
    var tableNames = applicationDbContext!.Model.GetEntityTypes()
        .Select(t => t.GetTableName())
        .Distinct()
        .ToList();

    foreach (var tableName in tableNames)
    {
#pragma warning disable EF1002
        applicationDbContext.Database.ExecuteSqlRaw($"TRUNCATE {tableName} CASCADE;");
#pragma warning restore EF1002
    }
}

async Task SeedDb(IServiceScope serviceScope,
    ApplicationDbContext? db)
{
    var registerManagerCommandHandler =
        serviceScope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();

    var manager = await registerManagerCommandHandler.Handle(new RegisterManagerCommand()
        { Email = new Email("jako1@jakob.se"), Name = new ManagerName("Jakob") });
    Console.WriteLine("Created manager with id: " + manager.Value?.Id);

    var createTeamCommandHandler =
        serviceScope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTeamCommand, Team>>();
    await createTeamCommandHandler.Handle(new CreateTeamCommand()
        { Name = new TeamName("Laget 2.0"), ManagerId = manager.Value!.Id });


    const int countriesToChooseFrom = Team.PlayerLimit / Team.PlayersFromSameCountryLimit;
    var goalkeepersRemaining = 1;
    var minDefendersRemaining = 4;
    var minMidfieldersRemaining = 4;

    static Player Player(Country country = Country.Se,
        Position position = Position.Defender)
    {
        return new Player(Guid.NewGuid(),
            new PlayerName("Jakob"),
            position,
            new CountryRec(country));
    }

    var players = Enumerable.Range(0, countriesToChooseFrom).SelectMany(i =>
        Enumerable.Range(0, Team.PlayersFromSameCountryLimit).Select(_ =>
        {
            Position position;
            if (goalkeepersRemaining > 0)
            {
                goalkeepersRemaining--;
                position = Position.Goalkeeper;
            }
            else if (minDefendersRemaining > 0)
            {
                minDefendersRemaining--;
                position = Position.Defender;
            }
            else if (minMidfieldersRemaining > 0)
            {
                minMidfieldersRemaining--;
                position = Position.Midfielder;
            }
            else
            {
                position = Position.Forward;
            }

            return Player((Country)i, position);
        })
    ).ToList();

    db!.Players.AddRange(players);
    await db.SaveChangesAsync();
}

// Make the implicit Program class public so test projects can access it
namespace ManagerGame
{
    public abstract class Program;
}
