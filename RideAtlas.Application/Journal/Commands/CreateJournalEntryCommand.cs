using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Journal.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Journal.Commands;

public record CreateJournalEntryCommand(
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

public class CreateJournalEntryCommandValidator : AbstractValidator<CreateJournalEntryCommand>
{
    public CreateJournalEntryCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public class CreateJournalEntryCommandHandler(IJournalEntryRepository repository)
    : IRequestHandler<CreateJournalEntryCommand, JournalEntryDto>
{
    public async Task<JournalEntryDto> Handle(CreateJournalEntryCommand request, CancellationToken ct)
    {
        var entry = new JournalEntry
        {
            RiderAccountId = request.RiderAccountId,
            BikeId = request.BikeId,
            TripId = request.TripId,
            RideId = request.RideId,
            Title = request.Title.Trim(),
            Date = request.Date,
            Notes = request.Notes,
            Mood = request.Mood,
            DistanceKm = request.DistanceKm,
            PhotoUrisCsv = JournalEntryMappingExtensions.JoinPhotoUris(request.PhotoUris),
            RiderInputRaw = request.RiderInputRaw
        };
        await repository.AddAsync(entry, ct);
        return entry.ToDto();
    }
}
