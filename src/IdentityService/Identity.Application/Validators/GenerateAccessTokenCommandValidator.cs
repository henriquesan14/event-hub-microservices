using FluentValidation;
using Identity.Application.Commands.GenerateAccessToken;

namespace Identity.Application.Validators;

public sealed class GenerateAccessTokenCommandValidator : AbstractValidator<GenerateAccessTokenCommand>
{
    public GenerateAccessTokenCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty()
            .WithMessage("O campo {PropertyName} é obrigatório");

        RuleFor(c => c.Password).NotEmpty()
            .WithMessage("O campo {PropertyName} é obrigatório");
    }
}
