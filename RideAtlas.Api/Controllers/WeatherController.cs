using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Weather.Dtos;
using RideAtlas.Application.Weather.Queries;

namespace RideAtlas.Api.Controllers;

public class WeatherController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("forecast-by-location")]
    public async Task<ActionResult<WeatherForecastDto>> ForecastByLocation(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] string? locationLabel,
        CancellationToken ct)
    {
        var forecast = await Sender.Send(new GetWeatherForecastQuery(latitude, longitude, locationLabel), ct);
        return Ok(forecast);
    }
}
