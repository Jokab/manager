using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class LeagueSettingsConfiguration : IEntityTypeConfiguration<LeagueSettings>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<LeagueSettings> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
