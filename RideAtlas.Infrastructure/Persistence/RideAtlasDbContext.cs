using Microsoft.EntityFrameworkCore;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence;

public class RideAtlasDbContext(DbContextOptions<RideAtlasDbContext> options) : DbContext(options)
{
    public DbSet<RiderAccount> RiderAccounts => Set<RiderAccount>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<Ride> Rides => Set<Ride>();
    public DbSet<FuelLogEntry> FuelLogEntries => Set<FuelLogEntry>();
    public DbSet<ServiceEntry> ServiceEntries => Set<ServiceEntry>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<PackingItem> PackingItems => Set<PackingItem>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<ReminderRule> ReminderRules => Set<ReminderRule>();
    public DbSet<ExplorerRide> ExplorerRides => Set<ExplorerRide>();
    public DbSet<RiderProfile> RiderProfiles => Set<RiderProfile>();
    public DbSet<RidingBuddy> RidingBuddies => Set<RidingBuddy>();
    public DbSet<CommunityPost> CommunityPosts => Set<CommunityPost>();
    public DbSet<CommunityComment> CommunityComments => Set<CommunityComment>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<RideEvent> RideEvents => Set<RideEvent>();
    public DbSet<EventInvitee> EventInvitees => Set<EventInvitee>();
    public DbSet<AppSettings> AppSettingsEntries => Set<AppSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RideAtlasDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
