using FluentValidation;
using MediatR;
using RideAtlas.Application.Auth.Dtos;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResultDto>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler(
    IRiderAccountRepository repository,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokenService,
    IDateTimeProvider clock) : IRequestHandler<LoginCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var account = await repository.GetByEmailAsync(normalizedEmail, ct);
        if (account is null || !passwordHasher.Verify(request.Password, account.PasswordHash))
            throw new AuthenticationFailedException("Invalid email or password.");

        return await AuthResultFactory.IssueAsync(account, repository, tokenService, clock, ct);
    }
}
