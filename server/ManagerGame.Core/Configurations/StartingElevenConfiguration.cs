using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class StartingElevenConfiguration : IEntityTypeConfiguration<StartingEleven>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<StartingEleven> builder)
    {
        // Keep table name stable even if DbSet properties are removed from DbContext.
        builder.ToTable("starting_elevens");
        builder.HasKey(x => x.Id);
    }
}
