using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Documents.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Documents.Commands;

public record CreateDocumentCommand(
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

public class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
{
    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.Type).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public class CreateDocumentCommandHandler(IDocumentRepository repository)
    : IRequestHandler<CreateDocumentCommand, DocumentDto>
{
    public async Task<DocumentDto> Handle(CreateDocumentCommand request, CancellationToken ct)
    {
        var document = new Document
        {
            RiderAccountId = request.RiderAccountId,
            BikeId = request.BikeId,
            Type = request.Type.Trim(),
            Title = request.Title.Trim(),
            DocumentNumber = request.DocumentNumber,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            ReminderEnabled = request.ReminderEnabled,
            PhotoUri = request.PhotoUri,
            PdfUri = request.PdfUri,
            Notes = request.Notes,
            Cost = request.Cost
        };
        await repository.AddAsync(document, ct);
        return document.ToDto();
    }
}
