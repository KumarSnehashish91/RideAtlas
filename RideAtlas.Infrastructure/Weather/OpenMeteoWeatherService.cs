using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Weather.Dtos;

namespace RideAtlas.Infrastructure.Weather;

public class OpenMeteoWeatherService(HttpClient httpClient) : IWeatherService
{
    public async Task<WeatherForecastDto> GetForecastAsync(double latitude, double longitude, string? locationLabel, CancellationToken ct = default)
    {
        var url =
            "/v1/forecast" +
            $"?latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
            $"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}" +
            "&current=temperature_2m,apparent_temperature,relative_humidity_2m,weather_code,wind_speed_10m,precipitation_probability" +
            "&hourly=temperature_2m,precipitation_probability" +
            "&daily=temperature_2m_max,temperature_2m_min,sunrise,sunset,precipitation_probability_max" +
            "&forecast_days=1&timezone=auto";

        var response = await httpClient.GetFromJsonAsync<OpenMeteoResponse>(url, ct)
            ?? throw new InvalidOperationException("Open-Meteo returned an empty response.");

        var condition = MapWeatherCode(response.Current.WeatherCode);
        var rainProbability = response.Current.PrecipitationProbability
            ?? response.Daily.PrecipitationProbabilityMax.FirstOrDefault();
        var windKmh = response.Current.WindSpeed10M;

        var hourly = response.Hourly.Time
            .Zip(response.Hourly.Temperature2M, response.Hourly.PrecipitationProbability)
            .Select(t => new HourlyForecastDto(t.First, t.Second, t.Third))
            .Take(24)
            .ToList();

        return new WeatherForecastDto(
            LocationLabel: locationLabel ?? $"{latitude:0.###}, {longitude:0.###}",
            TemperatureC: response.Current.Temperature2M,
            FeelsLikeC: response.Current.ApparentTemperature,
            Condition: condition,
            HighC: response.Daily.Temperature2MMax.FirstOrDefault(),
            LowC: response.Daily.Temperature2MMin.FirstOrDefault(),
            WindKmh: windKmh,
            HumidityPercent: response.Current.RelativeHumidity2M,
            RainProbabilityPercent: rainProbability,
            Sunrise: response.Daily.Sunrise.FirstOrDefault(),
            Sunset: response.Daily.Sunset.FirstOrDefault(),
            Hourly: hourly,
            Suitability: DeriveSuitability(response.Current.WeatherCode, windKmh, rainProbability));
    }

    private static string DeriveSuitability(int weatherCode, double windKmh, int rainProbability)
    {
        if (weatherCode >= 95) return "Extreme";
        if (rainProbability >= 60 || weatherCode is >= 51 and <= 82) return "Rain Expected";
        if (windKmh >= 40) return "High Wind";
        if (weatherCode <= 1 && windKmh < 20 && rainProbability < 20) return "Excellent";
        return "Good";
    }

    private static string MapWeatherCode(int code) => code switch
    {
        0 => "Clear sky",
        1 or 2 or 3 => "Partly cloudy",
        45 or 48 => "Fog",
        51 or 53 or 55 => "Drizzle",
        56 or 57 => "Freezing drizzle",
        61 or 63 or 65 => "Rain",
        66 or 67 => "Freezing rain",
        71 or 73 or 75 or 77 => "Snow",
        80 or 81 or 82 => "Rain showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorm",
        96 or 99 => "Thunderstorm with hail",
        _ => "Unknown"
    };

    private record OpenMeteoResponse(
        [property: JsonPropertyName("current")] CurrentBlock Current,
        [property: JsonPropertyName("hourly")] HourlyBlock Hourly,
        [property: JsonPropertyName("daily")] DailyBlock Daily);

    private record CurrentBlock(
        [property: JsonPropertyName("temperature_2m")] double Temperature2M,
        [property: JsonPropertyName("apparent_temperature")] double ApparentTemperature,
        [property: JsonPropertyName("relative_humidity_2m")] int RelativeHumidity2M,
        [property: JsonPropertyName("weather_code")] int WeatherCode,
        [property: JsonPropertyName("wind_speed_10m")] double WindSpeed10M,
        [property: JsonPropertyName("precipitation_probability")] int? PrecipitationProbability);

    private record HourlyBlock(
        [property: JsonPropertyName("time")] IReadOnlyList<DateTime> Time,
        [property: JsonPropertyName("temperature_2m")] IReadOnlyList<double> Temperature2M,
        [property: JsonPropertyName("precipitation_probability")] IReadOnlyList<int> PrecipitationProbability);

    private record DailyBlock(
        [property: JsonPropertyName("temperature_2m_max")] IReadOnlyList<double> Temperature2MMax,
        [property: JsonPropertyName("temperature_2m_min")] IReadOnlyList<double> Temperature2MMin,
        [property: JsonPropertyName("sunrise")] IReadOnlyList<DateTime> Sunrise,
        [property: JsonPropertyName("sunset")] IReadOnlyList<DateTime> Sunset,
        [property: JsonPropertyName("precipitation_probability_max")] IReadOnlyList<int> PrecipitationProbabilityMax);
}
