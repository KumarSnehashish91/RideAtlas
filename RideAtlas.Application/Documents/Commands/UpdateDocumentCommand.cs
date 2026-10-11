using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Documents.Dtos;

namespace RideAtlas.Application.Documents.Commands;

public record UpdateDocumentCommand(
    Guid Id,
    Guid RiderAccountId,
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
    decimal? Cost) : IRequest<DocumentDto>;

public class UpdateDocumentCommandValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        RuleFor(x => x.Type).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public class UpdateDocumentCommandHandler(IDocumentRepository repository)
    : IRequestHandler<UpdateDocumentCommand, DocumentDto>
{
    public async Task<DocumentDto> Handle(UpdateDocumentCommand request, CancellationToken ct)
    {
        var document = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Document), request.Id);

        document.BikeId = request.BikeId;
        document.Type = request.Type.Trim();
        document.Title = request.Title.Trim();
        document.DocumentNumber = request.DocumentNumber;
        document.IssueDate = request.IssueDate;
        document.ExpiryDate = request.ExpiryDate;
        document.ReminderEnabled = request.ReminderEnabled;
        document.PhotoUri = request.PhotoUri;
        document.PdfUri = request.PdfUri;
        document.Notes = request.Notes;
        document.Cost = request.Cost;

        await repository.UpdateAsync(document, ct);
        return document.ToDto();
    }
}
