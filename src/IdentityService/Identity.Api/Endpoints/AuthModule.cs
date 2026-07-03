using BuildingBlocks.Api.Extensions;
using Carter;
using FluentValidation;
using Identity.Application.Commands.GenerateAccessToken;
using Identity.Application.Commands.RegisterUser;
using MediatR;

namespace Identity.Api.Endpoints;

public sealed class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/", Login);
        group.MapPost("/register", Register);
    }

    private static async Task<IResult> Login(
        GenerateAccessTokenCommand command,
        IValidator<GenerateAccessTokenCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var validation = await validator.ValidateRequest(command, ct);

        if (validation is not null)
            return validation;

        var result = await sender.Send(command, ct);

        return result.ToHttpResult();
    }

    private static async Task<IResult> Register(
        RegisterUserCommand command,
        IValidator<RegisterUserCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var validation = await validator.ValidateRequest(command, ct);

        if (validation is not null)
            return validation;

        var result = await sender.Send(command, ct);

        return result.ToHttpResult();
    }
}
