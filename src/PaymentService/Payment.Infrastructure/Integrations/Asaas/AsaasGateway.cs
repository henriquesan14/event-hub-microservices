using System.Net.Http.Json;
using System.Text.Json;
using Payment.Application.Contracts;

namespace Payment.Infrastructure.Integrations.Asaas;

public sealed class AsaasGateway(HttpClient httpClient) : IAsaasGateway
{
    public async Task<AsaasChargeResult> CreateChargeAsync(
        CreateAsaasCharge request,
        CancellationToken ct)
    {
        var customerId = await FindCustomerAsync(request.UserId, ct)
            ?? await CreateCustomerAsync(request, ct);

        using var response = await httpClient.PostAsJsonAsync(
            "payments",
            new
            {
                customer = customerId,
                billingType = request.BillingType,
                value = request.Value,
                dueDate = request.DueDate.ToString("yyyy-MM-dd"),
                description = request.Description,
                externalReference = request.PaymentId.ToString()
            },
            ct);

        var charge = await ReadRequiredResponseAsync<AsaasPaymentResponse>(response, ct);
        if (string.IsNullOrWhiteSpace(charge.Id) || string.IsNullOrWhiteSpace(charge.InvoiceUrl))
            throw new HttpRequestException("Asaas returned an invalid payment response.");

        return new AsaasChargeResult(
            charge.Id,
            customerId,
            charge.BillingType ?? request.BillingType,
            charge.InvoiceUrl);
    }

    public async Task CancelChargeAsync(string providerPaymentId, CancellationToken ct)
    {
        using var response = await httpClient.DeleteAsync(
            $"payments/{Uri.EscapeDataString(providerPaymentId)}",
            ct);

        if (response.IsSuccessStatusCode ||
            response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return;

        var error = await response.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException(
            $"Asaas cancellation failed with status {(int)response.StatusCode}: {error}");
    }

    private async Task<string?> FindCustomerAsync(Guid userId, CancellationToken ct)
    {
        using var response = await httpClient.GetAsync(
            $"customers?externalReference={Uri.EscapeDataString(userId.ToString())}&limit=1",
            ct);
        var result = await ReadRequiredResponseAsync<AsaasListResponse<AsaasCustomerResponse>>(
            response,
            ct);
        return result.Data.FirstOrDefault()?.Id;
    }

    private async Task<string> CreateCustomerAsync(
        CreateAsaasCharge request,
        CancellationToken ct)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "customers",
            new
            {
                name = request.Name,
                email = request.Email,
                cpfCnpj = OnlyDigits(request.CpfCnpj),
                mobilePhone = string.IsNullOrWhiteSpace(request.MobilePhone)
                    ? null
                    : OnlyDigits(request.MobilePhone),
                externalReference = request.UserId.ToString(),
                notificationDisabled = true
            },
            ct);

        var customer = await ReadRequiredResponseAsync<AsaasCustomerResponse>(response, ct);
        return !string.IsNullOrWhiteSpace(customer.Id)
            ? customer.Id
            : throw new HttpRequestException("Asaas returned an invalid customer response.");
    }

    private static async Task<T> ReadRequiredResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken ct)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                $"Asaas request failed with status {(int)response.StatusCode}: {error}");
        }

        return await response.Content.ReadFromJsonAsync<T>(
                   new JsonSerializerOptions(JsonSerializerDefaults.Web),
                   ct)
               ?? throw new HttpRequestException("Asaas returned an empty response.");
    }

    private static string OnlyDigits(string value) =>
        new(value.Where(char.IsDigit).ToArray());

    private sealed record AsaasCustomerResponse(string Id);
    private sealed record AsaasPaymentResponse(
        string Id,
        string? BillingType,
        string InvoiceUrl);
    private sealed record AsaasListResponse<T>(IReadOnlyList<T> Data);
}
