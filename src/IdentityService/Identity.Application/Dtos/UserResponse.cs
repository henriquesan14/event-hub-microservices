using Identity.Domain.Enums;

namespace Identity.Application.Dtos;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    UserRole Role,
    bool EmailConfirmed,
    DateTime? CreatedAt,
    string? CreatedByName);
