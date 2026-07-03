using BuildingBlocks.SharedKernel.Result;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Extensions;

public static class ErrorExtensions
{
    public static IResult ToProblem(this Error error)
    {
        var statusCode = error.ErrorType switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.AccessUnAuthorized => StatusCodes.Status401Unauthorized,
            ErrorType.AccessForbidden => StatusCodes.Status403Forbidden,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(
            statusCode: statusCode,
            title: error.Description,
            detail: $"{error.Code} - {error.Description}");
    }
}
