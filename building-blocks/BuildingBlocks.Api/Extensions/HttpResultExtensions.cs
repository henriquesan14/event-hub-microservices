using BuildingBlocks.SharedKernel.Result;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Extensions;

public static class HttpResultExtensions
{
    public static IResult ToHttpResult(this Result result)
    {
        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: error => error.ToProblem());
    }

    public static IResult ToHttpResult<T>(this ResultT<T> result)
    {
        return result.Match(
            onSuccess: Results.Ok,
            onFailure: error => error.ToProblem());
    }
}
