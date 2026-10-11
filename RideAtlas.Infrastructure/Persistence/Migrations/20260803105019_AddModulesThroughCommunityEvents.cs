using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideAtlas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddModulesThroughCommunityEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettingsEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThemeMode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DistanceUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FuelUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaintenanceRemindersEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PreferredMapStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VisibleQuickActionNamesCsv = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettingsEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorRiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JournalEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Caption = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PhotoUrisCsv = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DistanceKm = table.Column<double>(type: "float", nullable: true),
                    Visibility = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BikeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReminderEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PhotoUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdfUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventInvitees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuddyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RsvpStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventInvitees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BikeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RideId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReceiptPhotoUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OdometerKm = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExplorerRides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Origin = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DistanceKm = table.Column<double>(type: "float", nullable: false),
                    EstimatedDurationHours = table.Column<double>(type: "float", nullable: false),
                    RecommendedTripDays = table.Column<int>(type: "int", nullable: false),
                    RoadType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BestSeason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScenicRating = table.Column<double>(type: "float", nullable: false),
                    PermitRequired = table.Column<bool>(type: "bit", nullable: false),
                    CampingAllowed = table.Column<bool>(type: "bit", nullable: false),
                    MinimumExperience = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PackingTemplate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HeroImageUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Highlights = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExplorerRides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FuelLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BikeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RideId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdometerKm = table.Column<double>(type: "float", nullable: false),
                    Litres = table.Column<double>(type: "float", nullable: false),
                    PricePerLitre = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsFullTank = table.Column<bool>(type: "bit", nullable: false),
                    FuelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Station = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StationLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiptPhotoUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelLogEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BikeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RideId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Mood = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DistanceKm = table.Column<double>(type: "float", nullable: true),
                    PhotoUrisCsv = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RiderInputRaw = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AiGeneratedStory = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPacked = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostLikes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostLikes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReminderRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BikeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IntervalKm = table.Column<double>(type: "float", nullable: true),
                    IntervalDays = table.Column<int>(type: "int", nullable: true),
                    BaselineOdometerKm = table.Column<double>(type: "float", nullable: false),
                    BaselineDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RideEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeetingPoint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BikeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CoverPhotoUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RideEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiderProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HomeCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AvatarUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RidingSinceYear = table.Column<int>(type: "int", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    ShowLocation = table.Column<bool>(type: "bit", nullable: false),
                    ShowStats = table.Column<bool>(type: "bit", nullable: false),
                    ShowBikes = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiderProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RidingBuddies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HomeCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AvatarUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsFollowing = table.Column<bool>(type: "bit", nullable: false),
                    RemoteRiderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RidingBuddies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BikeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OdometerKm = table.Column<double>(type: "float", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Garage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceTypesCsv = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WorkshopAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Mechanic = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LabourCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PartsCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InvoicePhotoUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarrantyClaim = table.Column<bool>(type: "bit", nullable: false),
                    ExpectedNextServiceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedNextServiceOdometerKm = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettingsEntries_RiderAccountId",
                table: "AppSettingsEntries",
                column: "RiderAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityComments_PostId",
                table: "CommunityComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPosts_RiderAccountId",
                table: "CommunityPosts",
                column: "RiderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_BikeId",
                table: "Documents",
                column: "BikeId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ExpiryDate",
                table: "Documents",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_RiderAccountId",
                table: "Documents",
                column: "RiderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EventInvitees_EventId",
                table: "EventInvitees",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BikeId",
                table: "Expenses",
                column: "BikeId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_RiderAccountId",
                table: "Expenses",
                column: "RiderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TripId",
                table: "Expenses",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorerRides_Slug",
                table: "ExplorerRides",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelLogEntries_BikeId",
                table: "FuelLogEntries",
                column: "BikeId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelLogEntries_RiderAccountId",
                table: "FuelLogEntries",
                column: "RiderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_RideId",
                table: "JournalEntries",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_RiderAccountId",
                table: "JournalEntries",
                column: "RiderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TripId",
                table: "JournalEntries",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingItems_RiderAccountId",
                table: "PackingItems",
                column: "RiderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingItems_TripId",
                table: "PackingItems",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_PostLikes_PostId_RiderAccountId",
                table: "PostLikes",
                columns: new[] { "PostId", "RiderAccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReminderRules_BikeId",
                table: "ReminderRules",
                column: "BikeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderRules_RiderAccountId",
                table: "ReminderRules",
                column: "RiderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RideEvents_RiderAccountId",
                table: "RideEvents",
                column: "RiderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RiderProfiles_RiderAccountId",
                table: "RiderProfiles",
                column: "RiderAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RidingBuddies_RiderAccountId",
                table: "RidingBuddies",
                column: "RiderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEntries_BikeId",
                table: "ServiceEntries",
                column: "BikeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEntries_RiderAccountId",
                table: "ServiceEntries",
                column: "RiderAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettingsEntries");

            migrationBuilder.DropTable(
                name: "CommunityComments");

            migrationBuilder.DropTable(
                name: "CommunityPosts");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "EventInvitees");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "ExplorerRides");

            migrationBuilder.DropTable(
                name: "FuelLogEntries");

            migrationBuilder.DropTable(
                name: "JournalEntries");

            migrationBuilder.DropTable(
                name: "PackingItems");

            migrationBuilder.DropTable(
                name: "PostLikes");

            migrationBuilder.DropTable(
                name: "ReminderRules");

            migrationBuilder.DropTable(
                name: "RideEvents");

            migrationBuilder.DropTable(
                name: "RiderProfiles");

            migrationBuilder.DropTable(
                name: "RidingBuddies");

            migrationBuilder.DropTable(
                name: "ServiceEntries");
        }
    }
}
