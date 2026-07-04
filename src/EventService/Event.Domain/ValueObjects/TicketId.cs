using BuildingBlocks.SharedKernel.Abstractions;

namespace Events.Domain.ValueObjects;

public sealed record TicketId(Guid Value)
{
    public static TicketId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("TicketId cannot be empty.");

        return new(value);
    }

    public static TicketId New() =>
        new(Guid.NewGuid());
}
