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

    public static Error InvalidRefreshToken() =>
        Error.AccessUnAuthorized("Auth.InvalidRefreshToken", "Refresh token inválido, expirado ou revogado");

    public static Error InvalidCurrentPassword() =>
        Error.AccessUnAuthorized("Auth.InvalidCurrentPassword", "A senha atual está incorreta");

    public static Error InvalidVerifiedToken() =>
        Error.AccessUnAuthorized("Auth.Unauthorized", $"Token de verificação inválido");

    public static Error EmailNotConfirmed() =>
        Error.AccessForbidden("Auth.EmailNotConfirmed", "Confirme seu e-mail antes de entrar");

    public static Error UserEmailNotFound(string email) =>
        Error.NotFound("Auth.Unauthorized", $"User with {email} not found");
}
