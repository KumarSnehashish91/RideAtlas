using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class RideConfiguration : IEntityTypeConfiguration<Ride>
{
    public void Configure(EntityTypeBuilder<Ride> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RouteDescription).HasMaxLength(500);
        builder.Property(x => x.WeatherCondition).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.HasIndex(x => x.RiderAccountId);
        builder.HasIndex(x => x.BikeId);
        builder.HasIndex(x => x.TripId);
    }
}
