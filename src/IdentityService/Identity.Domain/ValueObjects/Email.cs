using BuildingBlocks.SharedKernel.Abstractions;

namespace Identity.Domain.ValueObjects;

public sealed record Email(string Value)
{
    public static Email Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Email cannot be empty.");
        }
        if (!value.Contains('@'))
        {
            throw new DomainException("Email is not valid.");
        }
        return new Email(value.Trim().ToLowerInvariant());
    }

    public override string ToString() => Value;
}