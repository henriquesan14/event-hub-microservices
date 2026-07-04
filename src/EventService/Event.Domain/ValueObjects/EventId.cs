using BuildingBlocks.SharedKernel.Abstractions;

namespace Events.Domain.ValueObjects;

public sealed record EventId(Guid Value)
{
    public static EventId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("EventId cannot be empty.");

        return new(value);
    }

    public static EventId New() =>
        new(Guid.NewGuid());
}
