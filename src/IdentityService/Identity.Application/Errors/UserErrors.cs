using BuildingBlocks.SharedKernel.Result;

namespace Identity.Application.Errors;

public static class UserErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("User.NotFound", $"User with Id: {id} not found");

    public static Error Conflict(string email) =>
        Error.Conflict("User.Conflict", $"User with Email: {email} already exists");

    public static Error CreateFailure =>
        Error.Failure("User.CreateFailure", $"Something went wrong in creating User");

    public static Error UpdateFailure =>
        Error.Failure("User.UpdateFailure", $"Something went wrong in updating User");

    public static Error DeleteFailure =>
        Error.Failure("User.DeleteFailure", $"Something went wrong in deleting User");
}
