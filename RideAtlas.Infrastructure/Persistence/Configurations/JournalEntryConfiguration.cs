using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);
        builder.Property(x => x.Mood).HasMaxLength(50);
        builder.Property(x => x.PhotoUrisCsv).HasMaxLength(4000);
        builder.Property(x => x.RiderInputRaw).HasMaxLength(4000);
        builder.Property(x => x.AiGeneratedStory).HasMaxLength(8000);
        builder.HasIndex(x => x.RiderAccountId);
        builder.HasIndex(x => x.TripId);
        builder.HasIndex(x => x.RideId);
    }
}
