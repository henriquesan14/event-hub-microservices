namespace Notification.Infrastructure.Email;

public sealed class EmailLinksOptions
{
    public const string SectionName = "Email:Links";

    public string BaseUrl { get; init; } = "http://localhost:3000";
    public string ConfirmEmailPath { get; init; } = "/confirm-email";
    public string ResetPasswordPath { get; init; } = "/reset-password";
}
