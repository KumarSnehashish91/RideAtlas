using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Reminders.Queries;

public record MaintenanceReminderDto(Guid BikeId, string Category, string Title, string Detail, string Severity);

public record ComputedRemindersQuery(Guid RiderAccountId, Guid? BikeId) : IRequest<IReadOnlyList<MaintenanceReminderDto>>;

public class ComputedRemindersQueryHandler(
    IBikeRepository bikeRepository,
    IReminderRuleRepository reminderRuleRepository,
    IDocumentRepository documentRepository,
    IServiceEntryRepository serviceEntryRepository,
    IDateTimeProvider clock) : IRequestHandler<ComputedRemindersQuery, IReadOnlyList<MaintenanceReminderDto>>
{
    private const int DueSoonDays = 14;
    private const double DueSoonKm = 500;

    public async Task<IReadOnlyList<MaintenanceReminderDto>> Handle(ComputedRemindersQuery request, CancellationToken ct)
    {
        var bikes = (await bikeRepository.ListAsync(request.RiderAccountId, ct))
            .Where(b => request.BikeId is null || b.Id == request.BikeId)
            .ToList();
        var today = clock.Today;
        var reminders = new List<MaintenanceReminderDto>();

        foreach (var bike in bikes)
        {
            var rules = await reminderRuleRepository.ListForBikeAsync(bike.Id, request.RiderAccountId, ct);
            foreach (var rule in rules)
                AddRuleReminder(reminders, bike, rule, today);

            var latestService = (await serviceEntryRepository.ListAllForBikeAsync(request.RiderAccountId, bike.Id, ct))
                .OrderByDescending(s => s.ServiceDate)
                .FirstOrDefault();
            if (latestService is not null)
                AddServiceReminder(reminders, bike, latestService, today);
        }

        var documents = (await documentRepository.ListAsync(request.RiderAccountId, request.BikeId, ct))
            .Where(d => d.ReminderEnabled && d.ExpiryDate.HasValue);
        foreach (var document in documents)
            AddDocumentReminder(reminders, document, today);

        return reminders;
    }

    private static void AddRuleReminder(List<MaintenanceReminderDto> reminders, Motorcycle bike, ReminderRule rule, DateOnly today)
    {
        double? remainingKm = rule.IntervalKm is double intervalKm
            ? rule.BaselineOdometerKm + intervalKm - bike.CurrentOdometerKm
            : null;
        int? remainingDays = rule.IntervalDays is int intervalDays
            ? rule.BaselineDate.AddDays(intervalDays).DayNumber - today.DayNumber
            : null;

        var severity = Severity(remainingKm, remainingDays);
        if (severity is null) return;

        var title = string.IsNullOrWhiteSpace(rule.CustomLabel) ? rule.Category : rule.CustomLabel!;
        var detail = DetailText(remainingKm, remainingDays);
        reminders.Add(new MaintenanceReminderDto(bike.Id, rule.Category, title, detail, severity));
    }

    private static void AddServiceReminder(List<MaintenanceReminderDto> reminders, Motorcycle bike, ServiceEntry service, DateOnly today)
    {
        if (service.ExpectedNextServiceOdometerKm is null && service.ExpectedNextServiceDate is null)
            return;

        double? remainingKm = service.ExpectedNextServiceOdometerKm - bike.CurrentOdometerKm;
        int? remainingDays = service.ExpectedNextServiceDate is DateOnly dueDate
            ? dueDate.DayNumber - today.DayNumber
            : null;

        var severity = Severity(remainingKm, remainingDays);
        if (severity is null) return;

        reminders.Add(new MaintenanceReminderDto(bike.Id, "Service", "Next Service", DetailText(remainingKm, remainingDays), severity));
    }

    private static void AddDocumentReminder(List<MaintenanceReminderDto> reminders, Document document, DateOnly today)
    {
        var remainingDays = document.ExpiryDate!.Value.DayNumber - today.DayNumber;
        var severity = Severity(null, remainingDays);
        if (severity is null) return;

        var detail = remainingDays <= 0
            ? $"Expired on {document.ExpiryDate:yyyy-MM-dd}"
            : $"Expires on {document.ExpiryDate:yyyy-MM-dd} ({remainingDays} days)";
        reminders.Add(new MaintenanceReminderDto(document.BikeId ?? Guid.Empty, document.Type, document.Title, detail, severity));
    }

    private static string? Severity(double? remainingKm, int? remainingDays)
    {
        var overdue = remainingKm <= 0 || remainingDays <= 0;
        if (overdue) return "Overdue";

        var dueSoon = (remainingKm.HasValue && remainingKm <= DueSoonKm) || (remainingDays.HasValue && remainingDays <= DueSoonDays);
        return dueSoon ? "DueSoon" : null;
    }

    private static string DetailText(double? remainingKm, int? remainingDays)
    {
        var parts = new List<string>();
        if (remainingKm.HasValue) parts.Add(remainingKm <= 0 ? $"{-remainingKm:0} km overdue" : $"Due in {remainingKm:0} km");
        if (remainingDays.HasValue) parts.Add(remainingDays <= 0 ? $"{-remainingDays} days overdue" : $"Due in {remainingDays} days");
        return string.Join(" / ", parts);
    }
}
