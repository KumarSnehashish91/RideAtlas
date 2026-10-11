using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class PackingItemConfiguration : IEntityTypeConfiguration<PackingItem>
{
    public void Configure(EntityTypeBuilder<PackingItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Category).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.RiderAccountId);
        builder.HasIndex(x => x.TripId);
    }
}
