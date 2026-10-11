namespace RideAtlas.Application.Packing;

public record PackingTemplateItem(string Category, string Name);

/// <summary>Mirrors the mobile app's seeded packing templates so a "Log a Ride" trip on web
/// starts from the same checklist a rider would get on their phone.</summary>
public static class PackingTemplates
{
    public const string Camping = "Camping";
    public const string WeekendRide = "Weekend Ride";
    public const string LadakhRide = "Ladakh Ride";
    public const string DailyRide = "Daily Ride";

    private static readonly IReadOnlyList<PackingTemplateItem> CampingItems =
    [
        new("Riding Gear", "Helmet"),
        new("Riding Gear", "Riding Jacket"),
        new("Riding Gear", "Riding Gloves"),
        new("Camping", "Tent"),
        new("Camping", "Sleeping Bag"),
        new("Camping", "Sleeping Mat"),
        new("Camping", "Portable Stove"),
        new("Camping", "Headlamp"),
        new("Documents", "Driving License"),
        new("Documents", "Registration Certificate"),
        new("Tools", "Puncture Repair Kit"),
        new("Tools", "Basic Tool Kit"),
    ];

    private static readonly IReadOnlyList<PackingTemplateItem> WeekendRideItems =
    [
        new("Riding Gear", "Helmet"),
        new("Riding Gear", "Riding Jacket"),
        new("Riding Gear", "Riding Gloves"),
        new("Clothing", "Change of Clothes"),
        new("Clothing", "Rain Gear"),
        new("Documents", "Driving License"),
        new("Documents", "Registration Certificate"),
        new("Tools", "Puncture Repair Kit"),
    ];

    private static readonly IReadOnlyList<PackingTemplateItem> LadakhRideItems =
    [
        new("Riding Gear", "Helmet"),
        new("Riding Gear", "Riding Jacket"),
        new("Riding Gear", "Riding Gloves"),
        new("Riding Gear", "Thermal Layers"),
        new("Riding Gear", "Balaclava"),
        new("Camping", "Tent"),
        new("Camping", "Sleeping Bag (Sub-zero rated)"),
        new("Health", "Diamox / Altitude Medication"),
        new("Health", "First Aid Kit"),
        new("Documents", "Driving License"),
        new("Documents", "Registration Certificate"),
        new("Documents", "Inner Line Permit"),
        new("Tools", "Puncture Repair Kit"),
        new("Tools", "Extra Clutch/Brake Cable"),
        new("Tools", "Extra Fuel Can"),
    ];

    private static readonly IReadOnlyList<PackingTemplateItem> DailyRideItems =
    [
        new("Riding Gear", "Helmet"),
        new("Riding Gear", "Riding Gloves"),
        new("Documents", "Driving License"),
        new("Documents", "Registration Certificate"),
        new("Tools", "Puncture Repair Kit"),
    ];

    public static IReadOnlyList<PackingTemplateItem> Get(string templateName) => templateName switch
    {
        Camping => CampingItems,
        WeekendRide => WeekendRideItems,
        LadakhRide => LadakhRideItems,
        DailyRide => DailyRideItems,
        _ => throw new ArgumentOutOfRangeException(nameof(templateName), templateName, "Unknown packing template.")
    };

    public static readonly IReadOnlyList<string> Names = [Camping, WeekendRide, LadakhRide, DailyRide];
}
