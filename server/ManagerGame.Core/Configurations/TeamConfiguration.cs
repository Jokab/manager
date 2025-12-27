using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Team> builder)
    {
        builder.Property(x => x.Name)
            .HasConversion(x => x.Name, x => new TeamName(x));
        builder.HasMany(x => x.Players)
            .WithOne(tp => tp.Team)
            .HasForeignKey(tp => tp.TeamId)
            .IsRequired();
        builder.HasMany(x => x.StartingElevens)
            .WithOne(x => x.Team)
            .HasForeignKey(x => x.TeamId)
            .IsRequired();
    }
}
