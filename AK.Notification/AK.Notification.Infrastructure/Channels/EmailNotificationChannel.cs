using AK.Notification.Application.Channels;
using AK.Notification.Domain.Enums;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AK.Notification.Infrastructure.Channels;

internal sealed class EmailNotificationChannel(
    IOptions<EmailSettings> options,
    ILogger<EmailNotificationChannel> logger)
    : INotificationChannel
{
    public NotificationChannel Channel => NotificationChannel.Email;

    public async Task SendAsync(NotificationMessage message, CancellationToken ct = default)
    {
        var settings = options.Value;

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(settings.DisplayName, settings.From));
        email.To.Add(MailboxAddress.Parse(message.RecipientAddress));
        email.Subject = message.Subject ?? string.Empty;

        email.Body = new TextPart("plain") { Text = message.Body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(settings.Host, settings.Port, settings.EnableSsl, ct);

        if (!string.IsNullOrEmpty(settings.Username))
            await smtp.AuthenticateAsync(settings.Username, settings.Password, ct);

        await smtp.SendAsync(email, ct);
        await smtp.DisconnectAsync(true, ct);

        logger.LogInformation("Email sent to {Recipient} with subject '{Subject}'", message.RecipientAddress, message.Subject);
    }
}
