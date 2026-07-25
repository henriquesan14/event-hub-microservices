using FluentValidation;
using Ticketing.Application.Commands.CreateReservation;
using Ticketing.Application.Commands.CreateTicketType;
using Ticketing.Application.Commands.UpdateTicketType;

namespace Ticketing.Application.Validators;

public sealed class CreateTicketTypeCommandValidator : AbstractValidator<CreateTicketTypeCommand>
{
    public CreateTicketTypeCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.TotalQuantity).GreaterThan(0);
        RuleFor(x => x.SalesEnd).GreaterThan(x => x.SalesStart);
    }
}

public sealed class UpdateTicketTypeCommandValidator : AbstractValidator<UpdateTicketTypeCommand>
{
    public UpdateTicketTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.TotalQuantity).GreaterThan(0);
        RuleFor(x => x.SalesEnd).GreaterThan(x => x.SalesStart);
    }
}

public sealed class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.TicketTypeId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
    }
}
