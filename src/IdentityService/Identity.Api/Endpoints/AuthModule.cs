using BuildingBlocks.Api.Extensions;
using Carter;
using FluentValidation;
using Identity.Application.Commands.GenerateAccessToken;
using Identity.Application.Commands.Logout;
using Identity.Application.Commands.RefreshAccessToken;
using Identity.Application.Commands.RegisterUser;
using Identity.Application.Commands.ConfirmEmail;
using Identity.Application.Commands.ForgotPassword;
using Identity.Application.Commands.ResetPassword;
using Identity.Application.Commands.RequestEmailConfirmation;
using MediatR;

namespace Identity.Api.Endpoints;

public sealed class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/", Login);
        group.MapPost("/register", Register);
        group.MapPost("/refresh", Refresh);
        group.MapPost("/logout", Logout);
        group.MapPost("/confirm-email", ConfirmEmail);
        group.MapPost("/forgot-password", ForgotPassword);
        group.MapPost("/reset-password", ResetPassword);
        group.MapPost("/resend-confirmation", ResendConfirmation);
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

    private static async Task<IResult> Refresh(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new RefreshAccessTokenCommand(), ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> Logout(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new LogoutCommand(), ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> ConfirmEmail(
        ConfirmEmailCommand command,
        IValidator<ConfirmEmailCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var validation = await validator.ValidateRequest(command, ct);
        if (validation is not null) return validation;
        return (await sender.Send(command, ct)).ToHttpResult();
    }

    private static async Task<IResult> ForgotPassword(
        ForgotPasswordCommand command,
        IValidator<ForgotPasswordCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var validation = await validator.ValidateRequest(command, ct);
        if (validation is not null) return validation;
        return (await sender.Send(command, ct)).ToHttpResult();
    }

    private static async Task<IResult> ResetPassword(
        ResetPasswordCommand command,
        IValidator<ResetPasswordCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var validation = await validator.ValidateRequest(command, ct);
        if (validation is not null) return validation;
        return (await sender.Send(command, ct)).ToHttpResult();
    }

    private static async Task<IResult> ResendConfirmation(
        RequestEmailConfirmationCommand command,
        IValidator<RequestEmailConfirmationCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var validation = await validator.ValidateRequest(command, ct);
        if (validation is not null) return validation;
        return (await sender.Send(command, ct)).ToHttpResult();
    }
}
