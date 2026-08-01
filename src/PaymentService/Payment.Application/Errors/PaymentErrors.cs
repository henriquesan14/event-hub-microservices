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
    public static Error ChargeAlreadyCreated() =>
        Error.Conflict("Payment.ChargeAlreadyCreated", "An Asaas charge already exists for this payment");
    public static Error InvalidBillingType() =>
        Error.Validation("Payment.InvalidBillingType", "Billing type must be UNDEFINED, PIX, BOLETO or CREDIT_CARD");
    public static Error InvalidCheckoutData() =>
        Error.Validation("Payment.InvalidCheckoutData", "Name, email and CPF/CNPJ are required");
    public static Error UnsupportedCurrency(string currency) =>
        Error.Validation("Payment.UnsupportedCurrency", $"Asaas checkout requires BRL, but payment currency is {currency}");
    public static Error ProviderReferenceRequired() =>
        Error.Conflict("Payment.ProviderReferenceRequired", "Payment does not have an Asaas charge");
    public static Error BankSlipRefundUnsupported() =>
        Error.Validation("Payment.BankSlipRefundUnsupported", "Bank slip refunds require the Asaas bank details flow");
}
