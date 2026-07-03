using Identity.Application.Dtos;
using Identity.Domain.Entities;

namespace Identity.Application.Extensions;

public static class UserExtensions
{
    public static UserResponse ToDto(this User user)
    {
        return new UserResponse(
            user.Id.Value,
            user.Name,
            user.Email.Value,
            user.Role,
            user.CreatedAt,
            user.CreatedByName
        );
    }

    public static List<UserResponse> ToDto(this IEnumerable<User> users)
    {
        return users
            .Select(ToDto)
            .ToList();
    }
}
