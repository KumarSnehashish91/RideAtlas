using FluentValidation;
using MediatR;
using RideAtlas.Application.Bikes.Dtos;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Bikes.Commands;

public record UpdateBikeCommand(
    Guid Id,
    Guid RiderAccountId,
    string Nickname,
    string Brand,
    string Model,
    int Year,
    int EngineCc,
    string? RegistrationNumber,
    double CurrentOdometerKm,
    bool IsArchived) : IRequest<BikeDto>;

public class UpdateBikeCommandValidator : AbstractValidator<UpdateBikeCommand>
{
    public UpdateBikeCommandValidator()
    {
        RuleFor(x => x.Nickname).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100);
        RuleFor(x => x.EngineCc).GreaterThan(0);
        RuleFor(x => x.CurrentOdometerKm).GreaterThanOrEqualTo(0);
    }
}

public class UpdateBikeCommandHandler(IBikeRepository repository) : IRequestHandler<UpdateBikeCommand, BikeDto>
{
    public async Task<BikeDto> Handle(UpdateBikeCommand request, CancellationToken ct)
    {
        var bike = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Motorcycle), request.Id);

        bike.Nickname = request.Nickname.Trim();
        bike.Brand = request.Brand.Trim();
        bike.Model = request.Model.Trim();
        bike.Year = request.Year;
        bike.EngineCc = request.EngineCc;
        bike.RegistrationNumber = request.RegistrationNumber;
        bike.CurrentOdometerKm = request.CurrentOdometerKm;
        bike.IsArchived = request.IsArchived;

        await repository.UpdateAsync(bike, ct);
        return bike.ToDto();
    }
}
