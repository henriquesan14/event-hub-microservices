using BuildingBlocks.SharedKernel.Result;

namespace Identity.Application.Errors;

public static class AuthErrors
{
    public static Error Unauthorized() =>
        Error.AccessUnAuthorized("Auth.Unauthorized", $"Email/Password incorrect");

    public static Error SessionExpired() =>
        Error.AccessUnAuthorized("Auth.Unauthorized", $"Sua sessão expirou");
    public static Error RefreshTokenNotFound() =>
        Error.AccessUnAuthorized("Auth.Unauthorized", $"RefreshToken nao encontrado");

    public static Error InvalidVerifiedToken() =>
        Error.AccessUnAuthorized("Auth.Unauthorized", $"Token de verificação inválido");

    public static Error UserEmailNotFound(string email) =>
        Error.NotFound("Auth.Unauthorized", $"User with {email} not found");
}
