using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class RideEventConfiguration : IEntityTypeConfiguration<RideEvent>
{
    public void Configure(EntityTypeBuilder<RideEvent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.MeetingPoint).HasMaxLength(200);
        builder.Property(x => x.Destination).HasMaxLength(200);
        builder.HasIndex(x => x.RiderAccountId);
    }
}
