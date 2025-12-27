using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class MatchResultConfiguration : IEntityTypeConfiguration<MatchResult>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<MatchResult> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
