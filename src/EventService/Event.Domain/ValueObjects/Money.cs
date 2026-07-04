using BuildingBlocks.SharedKernel.Abstractions;

namespace Events.Domain.ValueObjects;

public sealed record Money(decimal Amount, string Currency)
{
    public static Money Of(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");

        return new(amount, currency);
    }
}
