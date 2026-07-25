namespace Payment.Application.Contracts;

public interface IAsaasGateway
{
    Task<AsaasChargeResult> CreateChargeAsync(
        CreateAsaasCharge request,
        CancellationToken ct);
    Task CancelChargeAsync(string providerPaymentId, CancellationToken ct);
}

public sealed record CreateAsaasCharge(
    Guid PaymentId,
    Guid UserId,
    string Name,
    string Email,
    string CpfCnpj,
    string? MobilePhone,
    string BillingType,
    decimal Value,
    DateTime DueDate,
    string Description);

public sealed record AsaasChargeResult(
    string PaymentId,
    string CustomerId,
    string BillingType,
    string InvoiceUrl);
