using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Api.Common;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.ServiceHistory.Commands;
using RideAtlas.Application.ServiceHistory.Dtos;
using RideAtlas.Application.ServiceHistory.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateServiceEntryRequest(
    Guid BikeId,
    DateOnly ServiceDate,
    double OdometerKm,
    decimal Cost,
    string? Garage,
    IReadOnlyList<string>? ServiceTypes,
    string Category,
    string? WorkshopAddress,
    string? Mechanic,
    string? Description,
    decimal? LabourCost,
    decimal? PartsCost,
    string? InvoiceNumber,
    string? InvoicePhotoUri,
    bool WarrantyClaim,
    DateOnly? ExpectedNextServiceDate,
    double? ExpectedNextServiceOdometerKm);

public record UpdateServiceEntryRequest(
    DateOnly ServiceDate,
    double OdometerKm,
    decimal Cost,
    string? Garage,
    IReadOnlyList<string>? ServiceTypes,
    string Category,
    string? WorkshopAddress,
    string? Mechanic,
    string? Description,
    decimal? LabourCost,
    decimal? PartsCost,
    string? InvoiceNumber,
    string? InvoicePhotoUri,
    bool WarrantyClaim,
    DateOnly? ExpectedNextServiceDate,
    double? ExpectedNextServiceOdometerKm);

public class ServiceHistoryController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ServiceEntryDto>>> List(
        [FromQuery] Guid? bikeId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(
            new ListServiceEntriesQuery(RiderAccountId, bikeId, fromDate, toDate, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceEntryDto>> Get(Guid id, CancellationToken ct)
    {
        var entry = await Sender.Send(new GetServiceEntryQuery(id, RiderAccountId), ct);
        return Ok(entry);
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<ServiceAnalyticsDto>> Analytics([FromQuery] Guid bikeId, CancellationToken ct)
    {
        var analytics = await Sender.Send(new ServiceAnalyticsQuery(RiderAccountId, bikeId), ct);
        return Ok(analytics);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceEntryDto>> Create(CreateServiceEntryRequest request, CancellationToken ct)
    {
        var entry = await Sender.Send(new CreateServiceEntryCommand(
            RiderAccountId, request.BikeId, request.ServiceDate, request.OdometerKm, request.Cost,
            request.Garage, request.ServiceTypes, request.Category, request.WorkshopAddress, request.Mechanic,
            request.Description, request.LabourCost, request.PartsCost, request.InvoiceNumber,
            request.InvoicePhotoUri, request.WarrantyClaim, request.ExpectedNextServiceDate,
            request.ExpectedNextServiceOdometerKm), ct);
        return Ok(entry);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ServiceEntryDto>> Update(Guid id, UpdateServiceEntryRequest request, CancellationToken ct)
    {
        var entry = await Sender.Send(new UpdateServiceEntryCommand(
            id, RiderAccountId, request.ServiceDate, request.OdometerKm, request.Cost,
            request.Garage, request.ServiceTypes, request.Category, request.WorkshopAddress, request.Mechanic,
            request.Description, request.LabourCost, request.PartsCost, request.InvoiceNumber,
            request.InvoicePhotoUri, request.WarrantyClaim, request.ExpectedNextServiceDate,
            request.ExpectedNextServiceOdometerKm), ct);
        return Ok(entry);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteServiceEntryCommand(id, RiderAccountId), ct);
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
            new ListServiceEntriesQuery(RiderAccountId, bikeId, fromDate, toDate, 1, int.MaxValue), ct);

        var headers = new[] { "Date", "Odometer (km)", "Cost", "Garage", "Category", "Service Types", "Notes" };
        var rows = result.Items.Select(e => (IReadOnlyList<string?>)
        [
            e.ServiceDate.ToString("yyyy-MM-dd"), e.OdometerKm.ToString("0.#"), e.Cost.ToString("0.##"),
            e.Garage, e.Category, string.Join(';', e.ServiceTypes), e.Description
        ]);

        var csv = CsvWriter.Write(headers, rows);
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "service-history.csv");
    }
}
