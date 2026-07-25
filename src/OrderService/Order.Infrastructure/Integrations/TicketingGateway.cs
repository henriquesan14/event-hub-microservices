using System.Net;
using System.Net.Http.Json;
using Order.Application.Contracts;

namespace Order.Infrastructure.Integrations;

public sealed class TicketingGateway(HttpClient client) : ITicketingGateway
{
    public async Task<ReservationSnapshot?> GetReservationAsync(Guid id, CancellationToken ct)
    {
        using var response = await client.GetAsync($"/api/reservations/{id}", ct);
        if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden
            or HttpStatusCode.Unauthorized)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReservationSnapshot>(cancellationToken: ct);
    }

    public async Task<bool> ReleaseReservationAsync(Guid id, CancellationToken ct)
    {
        using var response = await client.DeleteAsync($"/api/reservations/{id}", ct);
        return response.IsSuccessStatusCode;
    }
}
