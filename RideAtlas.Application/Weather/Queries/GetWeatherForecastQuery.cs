using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Weather.Dtos;

namespace RideAtlas.Application.Weather.Queries;

public record GetWeatherForecastQuery(double Latitude, double Longitude, string? LocationLabel) : IRequest<WeatherForecastDto>;

public class GetWeatherForecastQueryHandler(IWeatherService weatherService)
    : IRequestHandler<GetWeatherForecastQuery, WeatherForecastDto>
{
    public Task<WeatherForecastDto> Handle(GetWeatherForecastQuery request, CancellationToken ct) =>
        weatherService.GetForecastAsync(request.Latitude, request.Longitude, request.LocationLabel, ct);
}
