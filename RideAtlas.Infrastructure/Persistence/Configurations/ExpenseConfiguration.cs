using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Category).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Amount).HasColumnType("decimal(10,2)");
        builder.Property(x => x.Vendor).HasMaxLength(200);
        builder.Property(x => x.Location).HasMaxLength(200);
        builder.Property(x => x.PaymentMethod).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.HasIndex(x => x.RiderAccountId);
        builder.HasIndex(x => x.BikeId);
        builder.HasIndex(x => x.TripId);
    }
}
