using EventsApplication.Commands.CreateEvent;
using FluentValidation;

namespace EventsApplication.Validators;

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty()
            .WithMessage("O campo {PropertyName} é obrigatório")
            .MaximumLength(200).WithMessage("O campo {PropertyName} não pode ter mais de 200 caracteres");

        RuleFor(c => c.Description).NotEmpty()
            .WithMessage("O campo {PropertyName} é obrigatório")
            .MaximumLength(200).WithMessage("O campo {PropertyName} não pode ter mais de 200 caracteres");

        RuleFor(x => x.Address)
           .NotNull();

        RuleFor(x => x.StartsAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("O evento deve iniciar numa data futura.");

        RuleFor(x => x.EndsAt)
            .GreaterThan(x => x.StartsAt)
            .WithMessage("O final do evento tem que ser depois do inicio.");


        RuleFor(x => x.Address.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Address.Number)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Address.District)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Address.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Address.State)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Address.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Address.ZipCode)
            .NotEmpty()
            .MaximumLength(20);
    }
}
