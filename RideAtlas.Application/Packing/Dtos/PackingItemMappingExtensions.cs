using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Packing.Dtos;

public static class PackingItemMappingExtensions
{
    public static PackingItemDto ToDto(this PackingItem item) => new(
        item.Id,
        item.TripId,
        item.Category,
        item.Name,
        item.IsPacked,
        item.SortOrder,
        item.CreatedAt);
}
