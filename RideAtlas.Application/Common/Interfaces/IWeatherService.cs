using RideAtlas.Application.Weather.Dtos;

namespace RideAtlas.Application.Common.Interfaces;

/// <summary>Wraps Open-Meteo (no API key required) per §7.2, mirroring mobile's WeatherRepository shape.</summary>
public interface IWeatherService
{
    Task<WeatherForecastDto> GetForecastAsync(double latitude, double longitude, string? locationLabel, CancellationToken ct = default);
}
