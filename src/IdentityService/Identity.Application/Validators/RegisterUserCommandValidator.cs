using FluentValidation;
using Identity.Application.Commands.RegisterUser;

namespace Identity.Application.Validators;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty()
            .WithMessage("O campo {PropertyName} é obrigatório")
            .MaximumLength(200).WithMessage("O campo {PropertyName} não pode ter mais de 200 caracteres");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório")
            .EmailAddress().WithMessage("O campo {PropertyName} tem que ser um email válido")
            .MaximumLength(256).WithMessage("O campo {PropertyName} não pode ter mais de 256 caracteres");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório")
            .MinimumLength(6).WithMessage("O campo {PropertyName} não pode ter menos de 6 caracteres");
    }
}
