using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Journal.Dtos;

namespace RideAtlas.Application.Journal.Commands;

public record UpdateJournalEntryCommand(
    Guid Id,
    Guid RiderAccountId,
    Guid? BikeId,
    Guid? TripId,
    Guid? RideId,
    string Title,
    DateOnly Date,
    string? Notes,
    string? Mood,
    double? DistanceKm,
    IReadOnlyList<string>? PhotoUris,
    string? RiderInputRaw) : IRequest<JournalEntryDto>;

public class UpdateJournalEntryCommandValidator : AbstractValidator<UpdateJournalEntryCommand>
{
    public UpdateJournalEntryCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public class UpdateJournalEntryCommandHandler(IJournalEntryRepository repository)
    : IRequestHandler<UpdateJournalEntryCommand, JournalEntryDto>
{
    public async Task<JournalEntryDto> Handle(UpdateJournalEntryCommand request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.JournalEntry), request.Id);

        entry.BikeId = request.BikeId;
        entry.TripId = request.TripId;
        entry.RideId = request.RideId;
        entry.Title = request.Title.Trim();
        entry.Date = request.Date;
        entry.Notes = request.Notes;
        entry.Mood = request.Mood;
        entry.DistanceKm = request.DistanceKm;
        entry.PhotoUrisCsv = JournalEntryMappingExtensions.JoinPhotoUris(request.PhotoUris);
        entry.RiderInputRaw = request.RiderInputRaw;

        await repository.UpdateAsync(entry, ct);
        return entry.ToDto();
    }
}
