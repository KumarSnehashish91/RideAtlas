using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Packing.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Packing.Commands;

public record CreatePackingItemCommand(
    Guid RiderAccountId,
    Guid TripId,
    string Category,
    string Name,
    int SortOrder) : IRequest<PackingItemDto>;

public class CreatePackingItemCommandValidator : AbstractValidator<CreatePackingItemCommand>
{
    public CreatePackingItemCommandValidator()
    {
        RuleFor(x => x.TripId).NotEmpty();
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class CreatePackingItemCommandHandler(
    IPackingItemRepository repository,
    ITripRepository tripRepository) : IRequestHandler<CreatePackingItemCommand, PackingItemDto>
{
    public async Task<PackingItemDto> Handle(CreatePackingItemCommand request, CancellationToken ct)
    {
        _ = await tripRepository.GetByIdAsync(request.TripId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Trip), request.TripId);

        var item = new PackingItem
        {
            RiderAccountId = request.RiderAccountId,
            TripId = request.TripId,
            Category = request.Category.Trim(),
            Name = request.Name.Trim(),
            SortOrder = request.SortOrder
        };
        await repository.AddAsync(item, ct);
        return item.ToDto();
    }
}
