namespace Events.Domain.ValueObjects;

public sealed record Address(
    string Street,
    string Number,
    string District,
    string City,
    string State,
    string Country,
    string ZipCode);
