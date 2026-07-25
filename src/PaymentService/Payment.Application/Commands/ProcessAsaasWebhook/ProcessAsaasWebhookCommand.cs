using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Payment.Application.Commands.ProcessAsaasWebhook;

public sealed record ProcessAsaasWebhookCommand(
    string EventId,
    string EventType,
    string ProviderPaymentId) : ICommand<Result>;
