using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class ExplorerRideConfiguration : IEntityTypeConfiguration<ExplorerRide>
{
    public void Configure(EntityTypeBuilder<ExplorerRide> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Slug).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Country).HasMaxLength(100).IsRequired();
        builder.Property(x => x.State).HasMaxLength(100);
        builder.Property(x => x.Region).HasMaxLength(100);
        builder.Property(x => x.Origin).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Destination).HasMaxLength(200).IsRequired();
        builder.Property(x => x.RoadType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Difficulty).HasMaxLength(50).IsRequired();
        builder.Property(x => x.BestSeason).HasMaxLength(100);
        builder.Property(x => x.MinimumExperience).HasMaxLength(100);
        builder.Property(x => x.PackingTemplate).HasMaxLength(100);
        builder.Property(x => x.AiSummary).HasMaxLength(2000);
        builder.Property(x => x.Highlights).HasMaxLength(2000);
    }
}
