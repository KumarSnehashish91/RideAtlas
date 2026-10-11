using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class CommunityPostConfiguration : IEntityTypeConfiguration<CommunityPost>
{
    public void Configure(EntityTypeBuilder<CommunityPost> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Caption).HasMaxLength(2000);
        builder.Property(x => x.PhotoUrisCsv).HasMaxLength(4000);
        builder.Property(x => x.Location).HasMaxLength(200);
        builder.Property(x => x.Visibility).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.RiderAccountId);
    }
}
