using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class AppSettingsConfiguration : IEntityTypeConfiguration<AppSettings>
{
    public void Configure(EntityTypeBuilder<AppSettings> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ThemeMode).HasMaxLength(20).IsRequired();
        builder.Property(x => x.DistanceUnit).HasMaxLength(10).IsRequired();
        builder.Property(x => x.FuelUnit).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Language).HasMaxLength(20).IsRequired();
        builder.Property(x => x.PreferredMapStyle).HasMaxLength(50);
        builder.Property(x => x.VisibleQuickActionNamesCsv).HasMaxLength(500);
        builder.HasIndex(x => x.RiderAccountId).IsUnique();
    }
}
