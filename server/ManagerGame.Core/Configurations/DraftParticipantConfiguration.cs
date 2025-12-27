using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class DraftParticipantConfiguration : IEntityTypeConfiguration<DraftParticipant>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<DraftParticipant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.DraftId, x.Seat })
            .IsUnique();
        builder.HasIndex(x => new { x.DraftId, x.TeamId })
            .IsUnique();
        builder.HasOne<Draft>()
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.DraftId)
            .IsRequired();
    }
}
