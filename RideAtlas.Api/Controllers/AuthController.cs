using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Auth.Commands;
using RideAtlas.Application.Auth.Dtos;
using RideAtlas.Application.Auth.Queries;

namespace RideAtlas.Api.Controllers;

public record RegisterRequest(string FullName, string Email, string Password);
public record LoginRequest(string Email, string Password);

public class AuthController(ISender sender) : ApiControllerBase(sender)
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<RiderAccountDto>> Register(RegisterRequest request, CancellationToken ct)
    {
        var result = await Sender.Send(new RegisterCommand(request.FullName, request.Email, request.Password), ct);
        SetAuthCookies(result);
        return Ok(result.Account);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<RiderAccountDto>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await Sender.Send(new LoginCommand(request.Email, request.Password), ct);
        SetAuthCookies(result);
        return Ok(result.Account);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<RiderAccountDto>> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var result = await Sender.Send(new RefreshTokenCommand(refreshToken), ct);
        SetAuthCookies(result);
        return Ok(result.Account);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (!string.IsNullOrEmpty(refreshToken))
            await Sender.Send(new LogoutCommand(refreshToken), ct);

        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<ActionResult<RiderAccountDto>> Me(CancellationToken ct)
    {
        var result = await Sender.Send(new MeQuery(RiderAccountId), ct);
        return Ok(result);
    }

    private void SetAuthCookies(AuthResultDto result)
    {
        CookieOptions Options(DateTime expires) => new()
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            // SameSite=None requires Secure, or browsers/CookieContainer silently drop the cookie —
            // fall back to Lax over plain http (local dev, tests).
            SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Path = "/",
            Expires = expires
        };
        Response.Cookies.Append("access_token", result.AccessToken, Options(result.AccessTokenExpiresAt));
        Response.Cookies.Append("refresh_token", result.RefreshToken, Options(result.RefreshTokenExpiresAt));
    }
}
