using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Extensions;

public static class ValidationExtensions
{
    public static async Task<IResult?> ValidateRequest<T>(
        this IValidator<T> validator,
        T request,
        CancellationToken ct = default)
    {
        var result = await validator.ValidateAsync(request, ct);

        if (result.IsValid)
            return null;

        return Results.BadRequest(new
        {
            statusCode = StatusCodes.Status400BadRequest,
            message = result.Errors
                .Select(x => x.ErrorMessage)
                .ToList()
        });
    }
}