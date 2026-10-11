using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Common.Exceptions;

namespace RideAtlas.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (statusCode, title, errors) = Map(ex);
            if (statusCode == HttpStatusCode.InternalServerError)
                logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);

            var problem = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Type = $"https://httpstatuses.com/{(int)statusCode}"
            };
            if (errors is not null)
                problem.Extensions["errors"] = errors;

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private static (HttpStatusCode StatusCode, string Title, object? Errors) Map(Exception ex) => ex switch
    {
        NotFoundException => (HttpStatusCode.NotFound, ex.Message, null),
        ConflictException => (HttpStatusCode.Conflict, ex.Message, null),
        AuthenticationFailedException => (HttpStatusCode.Unauthorized, ex.Message, null),
        ValidationException vex => (HttpStatusCode.BadRequest, "One or more validation errors occurred.",
            vex.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),
        _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null)
    };
}
