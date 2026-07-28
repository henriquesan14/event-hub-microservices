using BuildingBlocks.SharedKernel.Result;

namespace Admission.Application.Errors;

public static class AdmissionErrors
{
    public static Error Unauthorized() =>
        Error.AccessUnAuthorized("Admission.Unauthorized", "Authentication is required.");

    public static Error NotFound() =>
        Error.NotFound("Admission.NotFound", "Ticket was not found.");

    public static Error Forbidden() =>
        Error.AccessForbidden("Admission.Forbidden", "You cannot access this ticket.");

    public static Error Invalid(string description) =>
        Error.Conflict("Admission.InvalidTicket", description);
}
