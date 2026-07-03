namespace BuildingBlocks.SharedKernel.Options;

public sealed class IpRateLimitingOptions
{
    public const string SectionName = "IpRateLimiting";

    public bool EnableEndpointRateLimiting { get; init; }
    public bool StackBlockedRequests { get; init; }
    public int HttpStatusCode { get; init; }
    public string RealIpHeader { get; init; } = default!;
    public string ClientIdHeader { get; init; } = default!;

    public List<RateLimitRule> GeneralRules { get; init; } = [];
}

public sealed class RateLimitRule
{
    public string Endpoint { get; init; } = default!;
    public string Period { get; init; } = default!;
    public int Limit { get; init; }
}
