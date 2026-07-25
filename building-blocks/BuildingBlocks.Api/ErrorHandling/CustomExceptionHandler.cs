using BuildingBlocks.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Api.ErrorHandling;

public sealed class CustomExceptionHandler
(ILogger<CustomExceptionHandler> logger, IHostEnvironment environment)
: IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception at {time}",
            DateTime.Now);

        (string Detail, string Title, int StatusCode) details = exception switch
        {
            DomainException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            _ =>
            (
                environment.IsDevelopment()
                    ? exception.GetBaseException().Message
                    : "An unexpected error occurred.",
                environment.IsDevelopment()
                    ? exception.GetType().Name
                    : "Internal Server Error",
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            )
        };

        var problemDetails = new ProblemDetails
        {
            Title = details.Title,
            Detail = details.Detail,
            Status = details.StatusCode,
            Instance = context.Request.Path
        };

        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}
