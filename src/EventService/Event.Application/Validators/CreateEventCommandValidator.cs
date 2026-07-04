using EventsApplication.Commands.CreateEvent;
using FluentValidation;

namespace EventsApplication.Validators;

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        
    }
}
