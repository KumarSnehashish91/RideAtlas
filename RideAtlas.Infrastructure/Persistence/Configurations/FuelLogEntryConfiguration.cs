using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class FuelLogEntryConfiguration : IEntityTypeConfiguration<FuelLogEntry>
{
    public void Configure(EntityTypeBuilder<FuelLogEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PricePerLitre).HasColumnType("decimal(10,2)");
        builder.Property(x => x.Cost).HasColumnType("decimal(10,2)");
        builder.Property(x => x.FuelType).HasMaxLength(50);
        builder.Property(x => x.Station).HasMaxLength(200);
        builder.Property(x => x.StationLocation).HasMaxLength(200);
        builder.Property(x => x.PaymentMethod).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.HasIndex(x => x.RiderAccountId);
        builder.HasIndex(x => x.BikeId);
    }
}
