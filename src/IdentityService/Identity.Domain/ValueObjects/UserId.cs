using BuildingBlocks.SharedKernel.Abstractions;

namespace Identity.Domain.ValueObjects;

public sealed record UserId(Guid Value)
{
    public static UserId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("UserId cannot be empty.");

        return new(value);
    }

    public static UserId New() =>
        new(Guid.NewGuid());
}
