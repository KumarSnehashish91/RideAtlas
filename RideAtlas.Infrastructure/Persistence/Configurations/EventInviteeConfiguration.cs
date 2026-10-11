using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Configurations;

public class EventInviteeConfiguration : IEntityTypeConfiguration<EventInvitee>
{
    public void Configure(EntityTypeBuilder<EventInvitee> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RsvpStatus).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.EventId);
    }
}
