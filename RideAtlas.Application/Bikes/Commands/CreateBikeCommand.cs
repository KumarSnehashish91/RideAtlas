using FluentValidation;
using MediatR;
using RideAtlas.Application.Bikes.Dtos;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Bikes.Commands;

public record CreateBikeCommand(
    Guid RiderAccountId,
    string Nickname,
    string Brand,
    string Model,
    int Year,
    int EngineCc,
    string? RegistrationNumber,
    double CurrentOdometerKm,
    bool IsActive) : IRequest<BikeDto>;

public class CreateBikeCommandValidator : AbstractValidator<CreateBikeCommand>
{
    public CreateBikeCommandValidator()
    {
        RuleFor(x => x.Nickname).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100);
        RuleFor(x => x.EngineCc).GreaterThan(0);
        RuleFor(x => x.CurrentOdometerKm).GreaterThanOrEqualTo(0);
    }
}

public class CreateBikeCommandHandler(IBikeRepository repository) : IRequestHandler<CreateBikeCommand, BikeDto>
{
    public async Task<BikeDto> Handle(CreateBikeCommand request, CancellationToken ct)
    {
        if (request.IsActive)
            await repository.ClearActiveFlagAsync(request.RiderAccountId, ct);

        var bike = new Motorcycle
        {
            RiderAccountId = request.RiderAccountId,
            Nickname = request.Nickname.Trim(),
            Brand = request.Brand.Trim(),
            Model = request.Model.Trim(),
            Year = request.Year,
            EngineCc = request.EngineCc,
            RegistrationNumber = request.RegistrationNumber,
            CurrentOdometerKm = request.CurrentOdometerKm,
            IsActive = request.IsActive
        };
        await repository.AddAsync(bike, ct);
        return bike.ToDto();
    }
}
