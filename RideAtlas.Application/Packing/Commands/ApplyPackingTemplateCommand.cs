using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Packing.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Packing.Commands;

public record ApplyPackingTemplateCommand(Guid RiderAccountId, Guid TripId, string TemplateName)
    : IRequest<IReadOnlyList<PackingItemDto>>;

public class ApplyPackingTemplateCommandValidator : AbstractValidator<ApplyPackingTemplateCommand>
{
    public ApplyPackingTemplateCommandValidator()
    {
        RuleFor(x => x.TripId).NotEmpty();
        RuleFor(x => x.TemplateName).Must(name => PackingTemplates.Names.Contains(name))
            .WithMessage($"Template must be one of: {string.Join(", ", PackingTemplates.Names)}");
    }
}

public class ApplyPackingTemplateCommandHandler(
    IPackingItemRepository repository,
    ITripRepository tripRepository) : IRequestHandler<ApplyPackingTemplateCommand, IReadOnlyList<PackingItemDto>>
{
    public async Task<IReadOnlyList<PackingItemDto>> Handle(ApplyPackingTemplateCommand request, CancellationToken ct)
    {
        _ = await tripRepository.GetByIdAsync(request.TripId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Trip), request.TripId);

        var templateItems = PackingTemplates.Get(request.TemplateName);
        var items = templateItems.Select((t, index) => new PackingItem
        {
            RiderAccountId = request.RiderAccountId,
            TripId = request.TripId,
            Category = t.Category,
            Name = t.Name,
            SortOrder = index
        }).ToList();

        await repository.AddRangeAsync(items, ct);
        return items.Select(i => i.ToDto()).ToList();
    }
}
