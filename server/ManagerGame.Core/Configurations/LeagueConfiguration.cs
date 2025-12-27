using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class LeagueConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<League> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Drafts)
            .WithOne(x => x.League).HasForeignKey(x => x.LeagueId)
            .IsRequired();
        builder.HasMany(x => x.Teams)
            .WithOne(x => x.League)
            .HasForeignKey(x => x.LeagueId)
            .IsRequired();
        builder.HasMany(x => x.MatchResults)
            .WithOne(x => x.League)
            .HasForeignKey(x => x.LeagueId)
            .IsRequired();
        builder.HasOne(x => x.Settings)
            .WithOne(x => x.League)
            .HasForeignKey<LeagueSettings>(x => x.LeagueId)
            .IsRequired();
    }
}
