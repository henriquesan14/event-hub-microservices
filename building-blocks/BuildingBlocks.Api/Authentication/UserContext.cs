using BuildingBlocks.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BuildingBlocks.Api.Authentication;

public sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value is string id
        ? Guid.Parse(id)
        : null;

    public string? Name => User?.FindFirst("name")?.Value;

    public bool IsInRole(string role) => User?.IsInRole(role) == true;

    public string? IpAddress
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                return forwardedFor.FirstOrDefault();
            }

            return httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }
    }

    public string? RefreshToken
    {
        get
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
            return refreshToken;
        }
    }

    public void SetCookieTokens(string accessToken, string refreshToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return;

        httpContext.Response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.Now.AddHours(1)
        });

        httpContext.Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.Now.AddDays(7)
        });
    }

    public void RemoveCookiesToken()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/",
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.None
        });

        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("access_token", new CookieOptions
        {
            Path = "/",
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.None
        });
    }
}
