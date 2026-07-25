using BuildingBlocks.SharedKernel.Result;

namespace Payment.Application.Errors;

public static class PaymentErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Payment.NotFound", $"Payment with Id: {id} was not found");
    public static Error OrderNotFound(Guid orderId) =>
        Error.NotFound("Payment.OrderNotFound", $"Payment for order: {orderId} was not found");
    public static Error Unauthorized() =>
        Error.AccessUnAuthorized("Payment.Unauthorized", "Authentication is required");
    public static Error Forbidden() =>
        Error.AccessForbidden("Payment.Forbidden", "This payment belongs to another user");
    public static Error InvalidState(string status) =>
        Error.Conflict("Payment.InvalidState", $"Payment cannot be changed from status: {status}");
}
