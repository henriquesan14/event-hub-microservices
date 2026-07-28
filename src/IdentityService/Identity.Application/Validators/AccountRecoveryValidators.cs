using FluentValidation;
using Identity.Application.Commands.ConfirmEmail;
using Identity.Application.Commands.ForgotPassword;
using Identity.Application.Commands.ResetPassword;
using Identity.Application.Commands.RequestEmailConfirmation;

namespace Identity.Application.Validators;

public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator() => RuleFor(x => x.Token).NotEmpty();
}

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator() =>
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
}

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
    }
}

public sealed class RequestEmailConfirmationCommandValidator
    : AbstractValidator<RequestEmailConfirmationCommand>
{
    public RequestEmailConfirmationCommandValidator() =>
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
}
