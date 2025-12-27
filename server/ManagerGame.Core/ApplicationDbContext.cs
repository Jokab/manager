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

    public DbSet<Manager> Managers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Draft> Drafts { get; set; }
    public DbSet<League> Leagues { get; set; }

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
