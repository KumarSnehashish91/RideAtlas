using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class ServiceEntryConfiguration : IEntityTypeConfiguration<ServiceEntry>
{
    public void Configure(EntityTypeBuilder<ServiceEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Cost).HasColumnType("decimal(10,2)");
        builder.Property(x => x.LabourCost).HasColumnType("decimal(10,2)");
        builder.Property(x => x.PartsCost).HasColumnType("decimal(10,2)");
        builder.Property(x => x.Garage).HasMaxLength(200);
        builder.Property(x => x.ServiceTypesCsv).HasMaxLength(500);
        builder.Property(x => x.Category).HasMaxLength(50).IsRequired();
        builder.Property(x => x.WorkshopAddress).HasMaxLength(300);
        builder.Property(x => x.Mechanic).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.InvoiceNumber).HasMaxLength(100);
        builder.HasIndex(x => x.RiderAccountId);
        builder.HasIndex(x => x.BikeId);
    }
}
