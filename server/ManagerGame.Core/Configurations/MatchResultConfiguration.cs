using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class MatchResultConfiguration : IEntityTypeConfiguration<MatchResult>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<MatchResult> builder)
    {
        // Keep table name stable even if DbSet properties are removed from DbContext.
        builder.ToTable("match_results");
        builder.HasKey(x => x.Id);
    }
}
