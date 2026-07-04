namespace Events.Application.Dtos;

public sealed record AddressRequest(
    string Street,
    string Number,
    string District,
    string City,
    string State,
    string Country,
    string ZipCode);
