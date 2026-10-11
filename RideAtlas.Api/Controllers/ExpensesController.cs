using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Api.Common;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Expenses.Commands;
using RideAtlas.Application.Expenses.Dtos;
using RideAtlas.Application.Expenses.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateExpenseRequest(
    Guid? BikeId,
    Guid? TripId,
    Guid? RideId,
    string Category,
    decimal Amount,
    DateOnly Date,
    string? Vendor,
    string? Location,
    string? PaymentMethod,
    string? Notes,
    string? ReceiptPhotoUri,
    double? OdometerKm);

public record UpdateExpenseRequest(
    Guid? BikeId,
    Guid? TripId,
    Guid? RideId,
    string Category,
    decimal Amount,
    DateOnly Date,
    string? Vendor,
    string? Location,
    string? PaymentMethod,
    string? Notes,
    string? ReceiptPhotoUri,
    double? OdometerKm);

public class ExpensesController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ExpenseDto>>> List(
        [FromQuery] Guid? bikeId,
        [FromQuery] Guid? tripId,
        [FromQuery] string? category,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(
            new ListExpensesQuery(RiderAccountId, bikeId, tripId, category, fromDate, toDate, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseDto>> Get(Guid id, CancellationToken ct)
    {
        var expense = await Sender.Send(new GetExpenseQuery(id, RiderAccountId), ct);
        return Ok(expense);
    }

    [HttpGet("category-breakdown")]
    public async Task<ActionResult<ExpenseCategoryBreakdownDto>> CategoryBreakdown(
        [FromQuery] Guid? bikeId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        CancellationToken ct = default)
    {
        var breakdown = await Sender.Send(
            new ExpenseCategoryBreakdownQuery(RiderAccountId, bikeId, fromDate, toDate), ct);
        return Ok(breakdown);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> Create(CreateExpenseRequest request, CancellationToken ct)
    {
        var expense = await Sender.Send(new CreateExpenseCommand(
            RiderAccountId, request.BikeId, request.TripId, request.RideId, request.Category,
            request.Amount, request.Date, request.Vendor, request.Location, request.PaymentMethod,
            request.Notes, request.ReceiptPhotoUri, request.OdometerKm), ct);
        return Ok(expense);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExpenseDto>> Update(Guid id, UpdateExpenseRequest request, CancellationToken ct)
    {
        var expense = await Sender.Send(new UpdateExpenseCommand(
            id, RiderAccountId, request.BikeId, request.TripId, request.RideId, request.Category,
            request.Amount, request.Date, request.Vendor, request.Location, request.PaymentMethod,
            request.Notes, request.ReceiptPhotoUri, request.OdometerKm), ct);
        return Ok(expense);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteExpenseCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] Guid? bikeId,
        [FromQuery] Guid? tripId,
        [FromQuery] string? category,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        CancellationToken ct)
    {
        var result = await Sender.Send(
            new ListExpensesQuery(RiderAccountId, bikeId, tripId, category, fromDate, toDate, 1, int.MaxValue), ct);

        var headers = new[] { "Date", "Category", "Amount", "Vendor", "Location", "Payment Method", "Notes" };
        var rows = result.Items.Select(e => (IReadOnlyList<string?>)
        [
            e.Date.ToString("yyyy-MM-dd"), e.Category, e.Amount.ToString("0.##"),
            e.Vendor, e.Location, e.PaymentMethod, e.Notes
        ]);

        var csv = CsvWriter.Write(headers, rows);
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "expenses.csv");
    }
}
