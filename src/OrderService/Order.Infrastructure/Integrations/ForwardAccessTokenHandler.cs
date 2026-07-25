using Microsoft.AspNetCore.Http;

namespace Order.Infrastructure.Integrations;

public sealed class ForwardAccessTokenHandler(IHttpContextAccessor accessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = accessor.HttpContext?.Request.Cookies["access_token"];
        if (!string.IsNullOrWhiteSpace(accessToken))
            request.Headers.TryAddWithoutValidation("Cookie", $"access_token={accessToken}");

        return base.SendAsync(request, cancellationToken);
    }
}
