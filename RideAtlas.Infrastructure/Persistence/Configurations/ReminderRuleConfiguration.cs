using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class ReminderRuleConfiguration : IEntityTypeConfiguration<ReminderRule>
{
    public void Configure(EntityTypeBuilder<ReminderRule> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Category).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CustomLabel).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => x.RiderAccountId);
        builder.HasIndex(x => x.BikeId);
    }
}
