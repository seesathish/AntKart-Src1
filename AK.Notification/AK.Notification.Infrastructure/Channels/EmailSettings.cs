namespace AK.Notification.Infrastructure.Channels;

public sealed record EmailSettings
{
    public string From { get; init; } = string.Empty;
    public string DisplayName { get; init; } = "AntKart";
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public bool EnableSsl { get; init; } = true;
    public string? Username { get; init; }
    public string? Password { get; init; }
}
