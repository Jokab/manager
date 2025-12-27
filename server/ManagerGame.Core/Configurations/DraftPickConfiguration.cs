using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class DraftPickConfiguration : IEntityTypeConfiguration<DraftPick>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<DraftPick> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.DraftId, x.PickNumber })
            .IsUnique();
        builder.HasIndex(x => new { x.DraftId, x.PlayerId })
            .IsUnique();
        builder.HasOne<Draft>()
            .WithMany(x => x.Picks)
            .HasForeignKey(x => x.DraftId)
            .IsRequired();
    }
}
