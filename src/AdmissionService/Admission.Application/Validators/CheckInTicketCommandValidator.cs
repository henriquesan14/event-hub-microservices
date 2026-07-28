using Admission.Application.Commands.CheckInTicket;
using FluentValidation;

namespace Admission.Application.Validators;

public sealed class CheckInTicketCommandValidator : AbstractValidator<CheckInTicketCommand>
{
    public CheckInTicketCommandValidator() =>
        RuleFor(x => x.Code).NotEmpty().MaximumLength(128);
}
