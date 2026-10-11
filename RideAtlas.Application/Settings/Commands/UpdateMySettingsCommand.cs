using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Settings.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Settings.Commands;

public record UpdateMySettingsCommand(
    Guid RiderAccountId,
    string ThemeMode,
    string DistanceUnit,
    string FuelUnit,
    string Currency,
    string Language,
    bool MaintenanceRemindersEnabled,
    string? PreferredMapStyle,
    IReadOnlyList<string>? VisibleQuickActionNames) : IRequest<AppSettingsDto>;

public class UpdateMySettingsCommandValidator : AbstractValidator<UpdateMySettingsCommand>
{
    private static readonly string[] AllowedThemes = ["Light", "Dark", "System"];
    private static readonly string[] AllowedDistanceUnits = ["km", "mi"];
    private static readonly string[] AllowedFuelUnits = ["L", "gal"];
    private static readonly string[] AllowedCurrencies = ["INR", "USD", "EUR", "GBP"];
    private static readonly string[] AllowedLanguages = ["English", "Hindi"];

    public UpdateMySettingsCommandValidator()
    {
        RuleFor(x => x.ThemeMode).Must(v => AllowedThemes.Contains(v));
        RuleFor(x => x.DistanceUnit).Must(v => AllowedDistanceUnits.Contains(v));
        RuleFor(x => x.FuelUnit).Must(v => AllowedFuelUnits.Contains(v));
        RuleFor(x => x.Currency).Must(v => AllowedCurrencies.Contains(v));
        RuleFor(x => x.Language).Must(v => AllowedLanguages.Contains(v));
    }
}

public class UpdateMySettingsCommandHandler(IAppSettingsRepository repository, IRiderAccountRepository riderAccountRepository)
    : IRequestHandler<UpdateMySettingsCommand, AppSettingsDto>
{
    public async Task<AppSettingsDto> Handle(UpdateMySettingsCommand request, CancellationToken ct)
    {
        var account = await riderAccountRepository.GetByIdAsync(request.RiderAccountId, ct);
        var riderName = account?.FullName ?? "Rider";

        var settings = await repository.GetByRiderAccountIdAsync(request.RiderAccountId, ct);
        if (settings is null)
        {
            settings = new AppSettings { RiderAccountId = request.RiderAccountId };
            Apply(settings, request);
            await repository.AddAsync(settings, ct);
        }
        else
        {
            Apply(settings, request);
            await repository.UpdateAsync(settings, ct);
        }
        return settings.ToDto(riderName);
    }

    private static void Apply(AppSettings settings, UpdateMySettingsCommand request)
    {
        settings.ThemeMode = request.ThemeMode;
        settings.DistanceUnit = request.DistanceUnit;
        settings.FuelUnit = request.FuelUnit;
        settings.Currency = request.Currency;
        settings.Language = request.Language;
        settings.MaintenanceRemindersEnabled = request.MaintenanceRemindersEnabled;
        settings.PreferredMapStyle = request.PreferredMapStyle;
        settings.VisibleQuickActionNamesCsv = AppSettingsMappingExtensions.JoinQuickActions(request.VisibleQuickActionNames);
    }
}
