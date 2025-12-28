using System.Text.Json.Serialization;
using ManagerGame;
using ManagerGame.Core;
using ManagerGame.Core.Drafting;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using ManagerGame.Hubs;
using ManagerGame.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Blazor Server services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddSignalR();

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<CurrentManagerService>();

RegisterCommandHandlers();

// Database configuration
if (builder.Environment.IsEnvironment("Test") || builder.Environment.IsEnvironment("Testing"))
{
    var connectionString =
        configuration["Test:Sqlite:ConnectionString"]
        ?? $"Data Source={Path.Combine(Path.GetTempPath(), "manager-test.db")}";
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("Db");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException(
            "Missing PostgreSQL connection string. Configure ConnectionStrings:Db (e.g. env var ConnectionStrings__Db).");

    builder.Services.AddNpgsql<ApplicationDbContext>(connectionString);
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // Fly.io handles HTTPS termination, so skip HSTS and HTTPS redirect in production
}
app.UseStaticFiles();
app.UseRouting();

// SignalR hub
app.MapHub<DraftHub>("/drafthub");

// Blazor routing
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Migrate command: supports --migrate-db (used by Fly release_command)
if (args.Contains("--migrate-db", StringComparer.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

    if (env.IsEnvironment("Testing") || env.IsEnvironment("Test"))
    {
        db.Database.EnsureCreated();
    }
    else
    {
        db.Database.Migrate();
    }

    return;
}

if (args.Contains("seed"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    ResetDb(db);
    await SeedDraftDebugData(scope, db);
}

app.Run();
return;

void RegisterCommandHandlers()
{
    AddCommandHandlerWithLogging<CreateTeamCommand, Team, CreateTeamCommandHandler>();
    AddCommandHandlerWithLogging<RegisterManagerCommand, Manager, RegisterManagerCommandHandler>();
    AddCommandHandlerWithLogging<LoginCommand, LoginResponse, LoginCommandHandler>();
    AddCommandHandlerWithLogging<CreateDraftRequest, Draft, CreateDraftHandler>();
    AddCommandHandlerWithLogging<StartDraftRequest, Draft, StartDraftHandler>();
    AddCommandHandlerWithLogging<PickDraftPlayerRequest, DraftPickOutcome, PickDraftPlayerHandler>();
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

async Task SeedDraftDebugData(IServiceScope serviceScope, ApplicationDbContext db)
{
    var registerHandler = serviceScope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();
    var createLeagueHandler = serviceScope.ServiceProvider.GetRequiredService<ICommandHandler<CreateLeagueRequest, League>>();
    var createTeamHandler = serviceScope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTeamCommand, Team>>();
    var createDraftHandler = serviceScope.ServiceProvider.GetRequiredService<ICommandHandler<CreateDraftRequest, Draft>>();

    // Create two managers
    var manager1Result = await registerHandler.Handle(new RegisterManagerCommand
    {
        Email = new Email("manager1@test.com"),
        Name = new ManagerName("Manager 1")
    });
    var manager2Result = await registerHandler.Handle(new RegisterManagerCommand
    {
        Email = new Email("manager2@test.com"),
        Name = new ManagerName("Manager 2")
    });

    Console.WriteLine($"Created Manager 1: {manager1Result.Value?.Id}");
    Console.WriteLine($"Created Manager 2: {manager2Result.Value?.Id}");

    // Create a league
    var leagueResult = await createLeagueHandler.Handle(new CreateLeagueRequest
    {
        Name = "Test Draft Liga"
    });
    var league = leagueResult.Value!;
    Console.WriteLine($"Created League: {league.Id} - {league.Name}");

    // Create teams for both managers in the league
    var team1Result = await createTeamHandler.Handle(new CreateTeamCommand
    {
        Name = new TeamName("Team Alpha"),
        ManagerId = manager1Result.Value!.Id,
        LeagueId = league.Id
    });
    var team2Result = await createTeamHandler.Handle(new CreateTeamCommand
    {
        Name = new TeamName("Team Beta"),
        ManagerId = manager2Result.Value!.Id,
        LeagueId = league.Id
    });

    Console.WriteLine($"Created Team 1: {team1Result.Value?.Id}");
    Console.WriteLine($"Created Team 2: {team2Result.Value?.Id}");

    // Seed players - enough for a draft (need 44+ for 2 teams with 22 each)
    var playerNames = new[]
    {
        "Emil Forsberg", "Alexander Isak", "Dejan Kulusevski", "Viktor Gyökeres", "Robin Olsen",
        "Ludwig Augustinsson", "Victor Lindelöf", "Kristoffer Olsson", "Sebastian Larsson", "Albin Ekdal",
        "Mikael Lustig", "Marcus Berg", "John Guidetti", "Oscar Hiljemark", "Ken Sema",
        "Pierre Højbjerg", "Christian Eriksen", "Kasper Schmeichel", "Simon Kjær", "Andreas Christensen",
        "Joachim Andersen", "Mikkel Damsgaard", "Rasmus Højlund", "Jonas Wind", "Yussuf Poulsen",
        "Thomas Delaney", "Daniel Wass", "Joakim Mæhle", "Jannik Vestergaard", "Robert Skov",
        "Joshua Kimmich", "Jamal Musiala", "Florian Wirtz", "Kai Havertz", "Leroy Sané",
        "Thomas Müller", "Antonio Rüdiger", "Jonathan Tah", "Niclas Füllkrug", "Manuel Neuer",
        "Erling Haaland", "Martin Ødegaard", "Sander Berge", "Jens Petter Hauge", "Morten Thorsby",
        "Pedri", "Gavi", "Lamine Yamal", "Nico Williams", "Dani Olmo",
        "Alvaro Morata", "Ferran Torres", "Rodri", "Unai Simón", "Marc Cucurella"
    };

    var countries = new[] { Country.Se, Country.Dk, Country.De, Country.No, Country.Es };

    var players = playerNames.Select((name, idx) =>
    {
        var country = countries[idx % countries.Length];
        // Distribute positions: ~10% GK, ~30% DEF, ~35% MID, ~25% FWD
        ManagerGame.Domain.Position position;
        var posRoll = idx % 20;
        if (posRoll < 2) position = ManagerGame.Domain.Position.Goalkeeper;
        else if (posRoll < 8) position = ManagerGame.Domain.Position.Defender;
        else if (posRoll < 15) position = ManagerGame.Domain.Position.Midfielder;
        else position = ManagerGame.Domain.Position.Forward;

        return new Player(new PlayerName(name), position, new CountryRec(country));
    }).ToList();

    db.Players.AddRange(players);
    await db.SaveChangesAsync();
    Console.WriteLine($"Seeded {players.Count} players");

    // Create a draft for the league
    var draftResult = await createDraftHandler.Handle(new CreateDraftRequest(league.Id));
    Console.WriteLine($"Created Draft: {draftResult.Value?.Id} (State: {draftResult.Value?.State})");

    Console.WriteLine("\n=== Debug Login Info ===");
    Console.WriteLine($"Manager 1: manager1@test.com");
    Console.WriteLine($"Manager 2: manager2@test.com");
    Console.WriteLine($"League: {league.Name} ({league.Id})");
}

// Make the implicit Program class public so test projects can access it
namespace ManagerGame
{
    public abstract class Program;
}
