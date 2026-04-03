using Maintenance_management.application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Maintenance_management.infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var emailSettings = _config.GetSection("EmailSettings");
        var host = emailSettings["SmtpHost"];
        var portStr = emailSettings["SmtpPort"];
        var user = emailSettings["SmtpUser"];
        var pass = emailSettings["SmtpPass"];
        var from = emailSettings["FromAddress"] ?? user;
        var fromName = emailSettings["FromName"] ?? "MaintenancePro";

        // If SMTP is not configured, log the reset link so it is usable in development
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user))
        {
            _logger.LogInformation(
                "[DEV] Password reset link for {Email}: {ResetLink}", toEmail, resetLink);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(from!, fromName),
            Subject = "Password Reset Request",
            Body = $"<p>You requested a password reset.</p>" +
                   $"<p>Click the link below to reset your password (valid for 1 hour):</p>" +
                   $"<p><a href=\"{resetLink}\">{resetLink}</a></p>" +
                   $"<p>If you did not request this, please ignore this email.</p>",
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        int port = int.TryParse(portStr, out var p) ? p : 587;
        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}
