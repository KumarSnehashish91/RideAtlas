using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Api.Common;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.FuelLog.Commands;
using RideAtlas.Application.FuelLog.Dtos;
using RideAtlas.Application.FuelLog.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateFuelLogEntryRequest(
    Guid BikeId,
    Guid? RideId,
    DateTime OccurredAt,
    double OdometerKm,
    double Litres,
    decimal PricePerLitre,
    decimal Cost,
    bool IsFullTank,
    string? FuelType,
    string? Station,
    string? StationLocation,
    string? PaymentMethod,
    string? ReceiptPhotoUri,
    string? Notes);

public record UpdateFuelLogEntryRequest(
    Guid? RideId,
    DateTime OccurredAt,
    double OdometerKm,
    double Litres,
    decimal PricePerLitre,
    decimal Cost,
    bool IsFullTank,
    string? FuelType,
    string? Station,
    string? StationLocation,
    string? PaymentMethod,
    string? ReceiptPhotoUri,
    string? Notes);

public class FuelLogController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<FuelLogEntryDto>>> List(
        [FromQuery] Guid? bikeId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(
            new ListFuelLogEntriesQuery(RiderAccountId, bikeId, fromDate, toDate, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FuelLogEntryDto>> Get(Guid id, CancellationToken ct)
    {
        var entry = await Sender.Send(new GetFuelLogEntryQuery(id, RiderAccountId), ct);
        return Ok(entry);
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<FuelAnalyticsDto>> Analytics([FromQuery] Guid bikeId, CancellationToken ct)
    {
        var analytics = await Sender.Send(new FuelAnalyticsQuery(RiderAccountId, bikeId), ct);
        return Ok(analytics);
    }

    [HttpPost]
    public async Task<ActionResult<FuelLogEntryDto>> Create(CreateFuelLogEntryRequest request, CancellationToken ct)
    {
        var entry = await Sender.Send(new CreateFuelLogEntryCommand(
            RiderAccountId, request.BikeId, request.RideId, request.OccurredAt, request.OdometerKm,
            request.Litres, request.PricePerLitre, request.Cost, request.IsFullTank,
            request.FuelType, request.Station, request.StationLocation, request.PaymentMethod,
            request.ReceiptPhotoUri, request.Notes), ct);
        return Ok(entry);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FuelLogEntryDto>> Update(Guid id, UpdateFuelLogEntryRequest request, CancellationToken ct)
    {
        var entry = await Sender.Send(new UpdateFuelLogEntryCommand(
            id, RiderAccountId, request.RideId, request.OccurredAt, request.OdometerKm,
            request.Litres, request.PricePerLitre, request.Cost, request.IsFullTank,
            request.FuelType, request.Station, request.StationLocation, request.PaymentMethod,
            request.ReceiptPhotoUri, request.Notes), ct);
        return Ok(entry);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteFuelLogEntryCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] Guid? bikeId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        CancellationToken ct)
    {
        var result = await Sender.Send(
            new ListFuelLogEntriesQuery(RiderAccountId, bikeId, fromDate, toDate, 1, int.MaxValue), ct);

        var headers = new[]
        {
            "Date", "Odometer (km)", "Litres", "Price/Litre", "Cost", "Full Tank", "Fuel Type",
            "Station", "Mileage (km/L)", "Cost/km", "Notes"
        };
        var rows = result.Items.Select(e => (IReadOnlyList<string?>)
        [
            e.OccurredAt.ToString("yyyy-MM-dd HH:mm"), e.OdometerKm.ToString("0.#"), e.Litres.ToString("0.##"),
            e.PricePerLitre.ToString("0.##"), e.Cost.ToString("0.##"), e.IsFullTank ? "Yes" : "No",
            e.FuelType, e.Station, e.MileageKmPerLitre?.ToString("0.##"), e.CostPerKm?.ToString("0.##"), e.Notes
        ]);

        var csv = CsvWriter.Write(headers, rows);
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "fuel-log.csv");
    }
}
