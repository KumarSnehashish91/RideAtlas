using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Packing.Dtos;

namespace RideAtlas.Application.Packing.Commands;

public record UpdatePackingItemCommand(
    Guid Id,
    Guid RiderAccountId,
    string Category,
    string Name,
    bool IsPacked,
    int SortOrder) : IRequest<PackingItemDto>;

public class UpdatePackingItemCommandValidator : AbstractValidator<UpdatePackingItemCommand>
{
    public UpdatePackingItemCommandValidator()
    {
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class UpdatePackingItemCommandHandler(IPackingItemRepository repository)
    : IRequestHandler<UpdatePackingItemCommand, PackingItemDto>
{
    public async Task<PackingItemDto> Handle(UpdatePackingItemCommand request, CancellationToken ct)
    {
        var item = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.PackingItem), request.Id);

        item.Category = request.Category.Trim();
        item.Name = request.Name.Trim();
        item.IsPacked = request.IsPacked;
        item.SortOrder = request.SortOrder;

        await repository.UpdateAsync(item, ct);
        return item.ToDto();
    }
}
