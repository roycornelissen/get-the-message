using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;

namespace CustomerServiceAgent;

internal class SmtpClientFactory(IConfiguration configuration)
{
    public async Task<SmtpClient> CreateAsync(CancellationToken cancellationToken = default)
    {
        var uri = configuration["SMTP_URI"];
        if (!string.IsNullOrWhiteSpace(uri))
        {
            var client = new SmtpClient();
            await client.ConnectAsync(new Uri(uri), cancellationToken);

            var smtpUser = configuration["Smtp:Username"];
            var smtpPassword = configuration["Smtp:Password"];
            if (!string.IsNullOrWhiteSpace(smtpUser) && !string.IsNullOrWhiteSpace(smtpPassword))
            {
                await client.AuthenticateAsync(smtpUser, smtpPassword, cancellationToken);
            }
            return client;
        }

        throw new InvalidOperationException("SMTP_URI configuration is missing or invalid.");
    }
}