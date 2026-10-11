namespace RideAtlas.Application.Weather.Dtos;

public record HourlyForecastDto(DateTime Time, double TemperatureC, int RainProbabilityPercent);

public record WeatherForecastDto(
    string LocationLabel,
    double TemperatureC,
    double FeelsLikeC,
    string Condition,
    double HighC,
    double LowC,
    double WindKmh,
    int HumidityPercent,
    int RainProbabilityPercent,
    DateTime? Sunrise,
    DateTime? Sunset,
    IReadOnlyList<HourlyForecastDto> Hourly,
    string Suitability);
