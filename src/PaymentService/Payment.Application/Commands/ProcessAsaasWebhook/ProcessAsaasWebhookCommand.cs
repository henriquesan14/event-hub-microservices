using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Payment.Application.Commands.ProcessAsaasWebhook;

public sealed record ProcessAsaasWebhookCommand(
    string EventId,
    string EventType,
    string ProviderPaymentId,
    string? BillingType) : ICommand<Result>;
