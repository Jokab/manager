using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public AggregateRepository<Draft, Guid> Drafts2 => new(
        Set<Draft>(),
        Set<Draft>()
            .Include(x => x.Participants)
            .Include(x => x.Picks),
        draft => draft.Id);

    public AggregateRepository<League, Guid> Leagues2 => new(
        Set<League>(),
        Set<League>()
            .Include(x => x.Teams)
            .Include(x => x.Settings)
            .Include(x => x.Drafts),
        league => league.Id);

    public AggregateRepository<Manager, Guid> Managers2 => new(
        Set<Manager>(),
        Set<Manager>().Include(x => x.Teams),
        manager => manager.Id);

    public AggregateRepository<Team, Guid> Teams2 => new(
        Set<Team>(),
        Set<Team>()
            .Include(x => x.Players)
            .ThenInclude(x => x.Player)
            .Include(x => x.League)
            .Include(x => x.StartingElevens)
            .ThenInclude(x => x.SelectedPlayers),
        team => team.Id);

    public AggregateRepository<Player, Guid> Players2 => new(
        Set<Player>(),
        Set<Player>()
            .Include(x => x.TeamPlayers)
            .Include(x => x.StartingElevenPlayers),
        player => player.Id);

    public AggregateRepository<TeamPlayer, Guid> TeamPlayers2 => new(
        Set<TeamPlayer>(),
        Set<TeamPlayer>().Include(x => x.Team).Include(x => x.Player),
        teamPlayer => teamPlayer.Id);

    public AggregateRepository<MatchResult, Guid> MatchResults2 => new(
        Set<MatchResult>(),
        Set<MatchResult>().Include(x => x.League).Include(x => x.MatchEvents),
        match => match.Id);

    public AggregateRepository<MatchEvent, Guid> MatchEvents2 => new(
        Set<MatchEvent>(),
        Set<MatchEvent>().Include(x => x.Match).Include(x => x.Player),
        matchEvent => matchEvent.Id);

    public AggregateRepository<StartingEleven, Guid> StartingElevens2 => new(
        Set<StartingEleven>(),
        Set<StartingEleven>().Include(x => x.Team).Include(x => x.SelectedPlayers),
        startingEleven => startingEleven.Id);

    public AggregateRepository<LeagueSettings, Guid> LeagueSettings2 => new(
        Set<LeagueSettings>(),
        Set<LeagueSettings>().Include(x => x.League),
        leagueSettings => leagueSettings.Id);

    public DbSet<Manager> Managers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Draft> Drafts { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<MatchResult> MatchResults { get; set; }
    public DbSet<MatchEvent> MatchEvents { get; set; }
    public DbSet<StartingEleven> StartingElevens { get; set; }
    public DbSet<LeagueSettings> LeagueSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure all Entity types to auto-generate Guid IDs
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(Entity.Id))
                    .ValueGeneratedOnAdd();
            }
        }

        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
