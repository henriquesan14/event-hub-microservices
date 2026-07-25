using EventsApplication.Commands.UpdateEvent;
using FluentValidation;

namespace EventsApplication.Validators;

public sealed class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Address).NotNull();
        RuleFor(x => x.StartsAt)
            .GreaterThan(DateTime.Now)
            .WithMessage("O evento deve iniciar numa data futura.");
        RuleFor(x => x.EndsAt)
            .GreaterThan(x => x.StartsAt)
            .WithMessage("O final do evento deve ser posterior ao início.");

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Address.Number).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Address.District).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address.State).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address.Country).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address.ZipCode).NotEmpty().MaximumLength(20);
        });
    }
}
