using System.Text;
using System.Text.Json.Serialization;
using ManagerGame;
using ManagerGame.Api;
using ManagerGame.Core;
using ManagerGame.Core.Drafting;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using ManagerGame.Infra;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddSignalR();

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
builder.Services.AddTransient<CreateLeagueHandler>();
builder.Services.AddTransient<AdmitTeamHandler>();

builder.Services.AddTransient<IRepository<Player>, Repository<Player>>();
builder.Services.AddTransient<IRepository<Team>, Repository<Team>>();
builder.Services.AddTransient<IRepository<Draft>, Repository<Draft>>();
builder.Services.AddTransient<IRepository<League>, Repository<League>>();

builder.Services.AddNpgsql<ApplicationDbContext>(builder.Configuration.GetConnectionString("Db"));

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapApi();

if (args.Contains("seed"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<ApplicationDbContext>();

    ResetDb(db);
    await SeedDb(scope, db);
}

app.Run();
return;

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
    var createManagerCommandHandler = serviceScope.ServiceProvider.GetService<CreateManagerCommandHandler>();
    var manager = createManagerCommandHandler!.Handle(new CreateManagerCommand
        { Email = new Email("jako1@jakob.se"), Name = new ManagerName("Jakob") });
    Console.WriteLine("Created manager with id: " + manager.Result.Value?.Id);

    var createTeamCommandHandler = serviceScope.ServiceProvider.GetService<CreateTeamCommandHandler>();
    await createTeamCommandHandler!.Handle(new CreateTeamCommand
        { Name = new TeamName("Laget 2.0"), ManagerId = manager.Result.Value!.Id });


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
