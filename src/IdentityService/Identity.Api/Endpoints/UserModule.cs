using BuildingBlocks.Api.Extensions;
using Carter;
using FluentValidation;
using Identity.Application.Commands.ChangePassword;
using Identity.Application.Commands.UpdateProfile;
using Identity.Application.Queries.GetCurrentUser;
using MediatR;

namespace Identity.Api.Endpoints;

public sealed class UserModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users/me")
            .RequireAuthorization();

        group.MapGet("/", GetCurrentUser);
        group.MapPut("/", UpdateProfile);
        group.MapPut("/password", ChangePassword);
    }

    private static async Task<IResult> GetCurrentUser(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetCurrentUserQuery(), ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateProfile(
        UpdateProfileCommand command,
        IValidator<UpdateProfileCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var validation = await validator.ValidateRequest(command, ct);
        if (validation is not null)
            return validation;

        var result = await sender.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> ChangePassword(
        ChangePasswordCommand command,
        IValidator<ChangePasswordCommand> validator,
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
