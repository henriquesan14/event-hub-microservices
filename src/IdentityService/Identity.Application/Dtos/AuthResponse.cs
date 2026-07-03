using Identity.Domain.Enums;

namespace Identity.Application.Dtos;

public sealed record AuthResponse(Guid UserId, string Name, UserRole Role);
