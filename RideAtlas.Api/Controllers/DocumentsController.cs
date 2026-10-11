using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Documents.Commands;
using RideAtlas.Application.Documents.Dtos;
using RideAtlas.Application.Documents.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateDocumentRequest(
    Guid? BikeId,
    string Type,
    string Title,
    string? DocumentNumber,
    DateOnly? IssueDate,
    DateOnly? ExpiryDate,
    bool ReminderEnabled,
    string? PhotoUri,
    string? PdfUri,
    string? Notes,
    decimal? Cost);

public record UpdateDocumentRequest(
    Guid? BikeId,
    string Type,
    string Title,
    string? DocumentNumber,
    DateOnly? IssueDate,
    DateOnly? ExpiryDate,
    bool ReminderEnabled,
    string? PhotoUri,
    string? PdfUri,
    string? Notes,
    decimal? Cost);

public class DocumentsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> List([FromQuery] Guid? bikeId, CancellationToken ct)
    {
        var documents = await Sender.Send(new ListDocumentsQuery(RiderAccountId, bikeId), ct);
        return Ok(documents);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DocumentDto>> Get(Guid id, CancellationToken ct)
    {
        var document = await Sender.Send(new GetDocumentQuery(id, RiderAccountId), ct);
        return Ok(document);
    }

    [HttpGet("expiring-soon")]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> ExpiringSoon([FromQuery] int withinDays = 30, CancellationToken ct = default)
    {
        var documents = await Sender.Send(new ListExpiringSoonDocumentsQuery(RiderAccountId, withinDays), ct);
        return Ok(documents);
    }

    [HttpPost]
    public async Task<ActionResult<DocumentDto>> Create(CreateDocumentRequest request, CancellationToken ct)
    {
        var document = await Sender.Send(new CreateDocumentCommand(
            RiderAccountId, request.BikeId, request.Type, request.Title, request.DocumentNumber,
            request.IssueDate, request.ExpiryDate, request.ReminderEnabled, request.PhotoUri,
            request.PdfUri, request.Notes, request.Cost), ct);
        return Ok(document);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DocumentDto>> Update(Guid id, UpdateDocumentRequest request, CancellationToken ct)
    {
        var document = await Sender.Send(new UpdateDocumentCommand(
            id, RiderAccountId, request.BikeId, request.Type, request.Title, request.DocumentNumber,
            request.IssueDate, request.ExpiryDate, request.ReminderEnabled, request.PhotoUri,
            request.PdfUri, request.Notes, request.Cost), ct);
        return Ok(document);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteDocumentCommand(id, RiderAccountId), ct);
        return NoContent();
    }
}
