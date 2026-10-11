using FluentValidation;
using MediatR;
using RideAtlas.Application.Auth.Dtos;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Auth.Commands;

public record RegisterCommand(string FullName, string Email, string Password) : IRequest<AuthResultDto>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public class RegisterCommandHandler(
    IRiderAccountRepository repository,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokenService,
    IDateTimeProvider clock) : IRequestHandler<RegisterCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await repository.GetByEmailAsync(normalizedEmail, ct) is not null)
            throw new ConflictException($"An account with email '{normalizedEmail}' already exists.");

        var account = new RiderAccount
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(request.Password)
        };
        await repository.AddAsync(account, ct);

        return await AuthResultFactory.IssueAsync(account, repository, tokenService, clock, ct);
    }
}
