using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class LeagueSettingsConfiguration : IEntityTypeConfiguration<LeagueSettings>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<LeagueSettings> builder)
    {
        // Keep table name stable even if DbSet properties are removed from DbContext.
        builder.ToTable("league_settings");
        builder.HasKey(x => x.Id);
    }
}
