using Maintenance_management.application.DTOs.Quotation;
using Maintenance_management.application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

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

    private (string? host, int port, string? user, string? pass, string from, string fromName) GetSmtpSettings()
    {
        var emailSettings = _config.GetSection("EmailSettings");
        var host = emailSettings["SmtpHost"];
        var portStr = emailSettings["SmtpPort"];
        var user = emailSettings["SmtpUser"];
        var pass = emailSettings["SmtpPass"];
        var from = emailSettings["FromAddress"] ?? user ?? "noreply@maintenancepro.com";
        var fromName = emailSettings["FromName"] ?? "MaintenancePro";
        int port = int.TryParse(portStr, out var p) ? p : 587;
        return (host, port, user, pass, from, fromName);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var smtp = GetSmtpSettings();

        // If SMTP is not configured, log the reset link so it is usable in development
        if (string.IsNullOrWhiteSpace(smtp.host) || string.IsNullOrWhiteSpace(smtp.user))
        {
            // Sanitize user-supplied values to prevent log forging (CRLF injection)
            var safeEmail = toEmail.Replace("\r", "", StringComparison.Ordinal)
                                   .Replace("\n", "", StringComparison.Ordinal);
            var safeLink = resetLink.Replace("\r", "", StringComparison.Ordinal)
                                    .Replace("\n", "", StringComparison.Ordinal);
            _logger.LogInformation(
                "[DEV] Password reset link for {Email}: {ResetLink}", safeEmail, safeLink);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(smtp.from, smtp.fromName),
            Subject = "Password Reset Request",
            Body = $"<p>You requested a password reset.</p>" +
                   $"<p>Click the link below to reset your password (valid for 1 hour):</p>" +
                   $"<p><a href=\"{resetLink}\">{resetLink}</a></p>" +
                   $"<p>If you did not request this, please ignore this email.</p>",
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(smtp.host, smtp.port)
        {
            Credentials = new NetworkCredential(smtp.user, smtp.pass),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }

    public async Task SendQuotationEmailAsync(string toEmail, QuotationDto quotation)
    {
        var smtp = GetSmtpSettings();

        if (string.IsNullOrWhiteSpace(smtp.host) || string.IsNullOrWhiteSpace(smtp.user))
        {
            var safeEmail = toEmail.Replace("\r", "", StringComparison.Ordinal)
                                   .Replace("\n", "", StringComparison.Ordinal);
            _logger.LogInformation(
                "[DEV] Quotation email for {Email}: {QuotationNumber}", safeEmail, quotation.QuotationNumber);
            return;
        }

        var body = BuildQuotationEmailBody(quotation);

        using var message = new MailMessage
        {
            From = new MailAddress(smtp.from, smtp.fromName),
            Subject = $"Quotation {Encode(quotation.QuotationNumber)} from MaintenancePro",
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(smtp.host, smtp.port)
        {
            Credentials = new NetworkCredential(smtp.user, smtp.pass),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }

    private static string Encode(string value)
        => System.Web.HttpUtility.HtmlEncode(value);

    private static string FormatCurrency(decimal value)
        => value.ToString("N2", CultureInfo.InvariantCulture);

    private static string BuildQuotationEmailBody(QuotationDto q)
    {
        var sb = new StringBuilder();

        // Build line items rows
        var lineItemsHtml = new StringBuilder();
        int idx = 1;
        foreach (var li in q.LineItems)
        {
            lineItemsHtml.Append(CultureInfo.InvariantCulture, $@"
            <tr style=""border-bottom:1px solid #e5e7eb;"">
              <td style=""padding:12px 16px;color:#374151;"">{idx++}</td>
              <td style=""padding:12px 16px;color:#374151;"">{Encode(li.Description)}</td>
              <td style=""padding:12px 16px;text-align:center;color:#374151;"">{FormatCurrency(li.Quantity)}</td>
              <td style=""padding:12px 16px;text-align:right;color:#374151;"">{FormatCurrency(li.UnitPrice)}</td>
              <td style=""padding:12px 16px;text-align:right;color:#374151;font-weight:600;"">{FormatCurrency(li.Total)}</td>
            </tr>");
        }

        var statusColor = q.Status switch
        {
            domain.Enums.QuotationStatus.Draft => "#6b7280",
            domain.Enums.QuotationStatus.Sent => "#2563eb",
            domain.Enums.QuotationStatus.Accepted => "#059669",
            domain.Enums.QuotationStatus.Rejected => "#dc2626",
            domain.Enums.QuotationStatus.Expired => "#d97706",
            domain.Enums.QuotationStatus.Cancelled => "#6b7280",
            _ => "#6b7280"
        };

        sb.Append(CultureInfo.InvariantCulture, $@"<!DOCTYPE html>
<html lang=""en"">
<head><meta charset=""UTF-8""><meta name=""viewport"" content=""width=device-width,initial-scale=1.0""></head>
<body style=""margin:0;padding:0;background-color:#f3f4f6;font-family:'Segoe UI',Roboto,Arial,sans-serif;"">
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f3f4f6;padding:32px 0;"">
  <tr><td align=""center"">
    <table width=""680"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);"">
      <!-- Header -->
      <tr>
        <td style=""background:linear-gradient(135deg,#1e40af 0%,#3b82f6 100%);padding:32px 40px;"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
            <tr>
              <td>
                <table cellpadding=""0"" cellspacing=""0"">
                  <tr>
                    <td style=""padding-right:12px;vertical-align:middle;"">
                      <!-- Wrench/Gear Icon -->
                      <svg xmlns=""http://www.w3.org/2000/svg"" width=""40"" height=""40"" viewBox=""0 0 24 24"" fill=""none"" stroke=""#ffffff"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><circle cx=""12"" cy=""12"" r=""3""/><path d=""M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06A1.65 1.65 0 0 0 4.68 15a1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06A1.65 1.65 0 0 0 9 4.68a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06A1.65 1.65 0 0 0 19.4 9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z""/></svg>
                    </td>
                    <td style=""vertical-align:middle;"">
                      <h1 style=""margin:0;font-size:26px;font-weight:700;color:#ffffff;letter-spacing:-0.5px;"">MaintenancePro</h1>
                      <p style=""margin:4px 0 0;font-size:13px;color:rgba(255,255,255,0.8);"">Professional Maintenance Management</p>
                    </td>
                  </tr>
                </table>
              </td>
              <td align=""right"" style=""vertical-align:middle;"">
                <span style=""display:inline-block;background:rgba(255,255,255,0.2);border:1px solid rgba(255,255,255,0.3);border-radius:20px;padding:6px 16px;font-size:13px;color:#ffffff;font-weight:600;letter-spacing:0.5px;"">QUOTATION</span>
              </td>
            </tr>
          </table>
        </td>
      </tr>

      <!-- Quotation Info Bar -->
      <tr>
        <td style=""background:#eff6ff;padding:20px 40px;border-bottom:1px solid #dbeafe;"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
            <tr>
              <td style=""width:50%;"">
                <p style=""margin:0;font-size:12px;color:#6b7280;text-transform:uppercase;letter-spacing:0.5px;"">Quotation Number</p>
                <p style=""margin:4px 0 0;font-size:18px;font-weight:700;color:#1e40af;"">{Encode(q.QuotationNumber)}</p>
              </td>
              <td style=""width:50%;text-align:right;"">
                <span style=""display:inline-block;background:{statusColor};color:#ffffff;border-radius:20px;padding:5px 16px;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.5px;"">{Encode(q.Status.ToString())}</span>
              </td>
            </tr>
          </table>
        </td>
      </tr>

      <!-- Client & Dates Info -->
      <tr>
        <td style=""padding:28px 40px 0;"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
            <tr>
              <td style=""width:50%;vertical-align:top;padding-right:20px;"">
                <p style=""margin:0 0 12px;font-size:12px;font-weight:600;color:#6b7280;text-transform:uppercase;letter-spacing:0.5px;"">Bill To</p>
                <p style=""margin:0;font-size:16px;font-weight:700;color:#111827;"">{Encode(q.ClientName)}</p>
                {(string.IsNullOrEmpty(q.ClientCompany) ? "" : $@"<p style=""margin:4px 0 0;font-size:14px;color:#4b5563;"">{Encode(q.ClientCompany)}</p>")}
                {(string.IsNullOrEmpty(q.ClientEmail) ? "" : $@"<p style=""margin:4px 0 0;font-size:14px;color:#4b5563;"">{Encode(q.ClientEmail)}</p>")}
                {(string.IsNullOrEmpty(q.ClientPhone) ? "" : $@"<p style=""margin:4px 0 0;font-size:14px;color:#4b5563;"">{Encode(q.ClientPhone)}</p>")}
                {(string.IsNullOrEmpty(q.ClientAddress) ? "" : $@"<p style=""margin:4px 0 0;font-size:14px;color:#4b5563;"">{Encode(q.ClientAddress)}</p>")}
              </td>
              <td style=""width:50%;vertical-align:top;"">
                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f9fafb;border-radius:8px;padding:16px;"">
                  <tr>
                    <td style=""padding:4px 0;"">
                      <span style=""font-size:12px;color:#6b7280;"">Issue Date</span><br/>
                      <span style=""font-size:14px;font-weight:600;color:#111827;"">{q.IssueDate:MMM dd, yyyy}</span>
                    </td>
                  </tr>
                  <tr>
                    <td style=""padding:4px 0;"">
                      <span style=""font-size:12px;color:#6b7280;"">Valid Until</span><br/>
                      <span style=""font-size:14px;font-weight:600;color:{(q.ValidUntil < DateTime.UtcNow ? "#dc2626" : "#111827")};"">{q.ValidUntil:MMM dd, yyyy}</span>
                    </td>
                  </tr>
                  <tr>
                    <td style=""padding:4px 0;"">
                      <span style=""font-size:12px;color:#6b7280;"">Est. Duration</span><br/>
                      <span style=""font-size:14px;font-weight:600;color:#111827;"">{q.EstimatedDurationDays} day{(q.EstimatedDurationDays != 1 ? "s" : "")}</span>
                    </td>
                  </tr>
                  {(q.MaintenanceRequestTitle is null ? "" : $@"<tr>
                    <td style=""padding:4px 0;"">
                      <span style=""font-size:12px;color:#6b7280;"">Related Request</span><br/>
                      <span style=""font-size:14px;font-weight:600;color:#1e40af;"">{Encode(q.MaintenanceRequestTitle)}</span>
                    </td>
                  </tr>")}
                </table>
              </td>
            </tr>
          </table>
        </td>
      </tr>

      <!-- Line Items Table -->
      <tr>
        <td style=""padding:28px 40px;"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;"">
            <thead>
              <tr style=""background:#f9fafb;"">
                <th style=""padding:12px 16px;text-align:left;font-size:12px;font-weight:600;color:#6b7280;text-transform:uppercase;letter-spacing:0.5px;border-bottom:2px solid #e5e7eb;"">#</th>
                <th style=""padding:12px 16px;text-align:left;font-size:12px;font-weight:600;color:#6b7280;text-transform:uppercase;letter-spacing:0.5px;border-bottom:2px solid #e5e7eb;"">Description</th>
                <th style=""padding:12px 16px;text-align:center;font-size:12px;font-weight:600;color:#6b7280;text-transform:uppercase;letter-spacing:0.5px;border-bottom:2px solid #e5e7eb;"">Qty</th>
                <th style=""padding:12px 16px;text-align:right;font-size:12px;font-weight:600;color:#6b7280;text-transform:uppercase;letter-spacing:0.5px;border-bottom:2px solid #e5e7eb;"">Unit Price</th>
                <th style=""padding:12px 16px;text-align:right;font-size:12px;font-weight:600;color:#6b7280;text-transform:uppercase;letter-spacing:0.5px;border-bottom:2px solid #e5e7eb;"">Total</th>
              </tr>
            </thead>
            <tbody>
              {lineItemsHtml}
            </tbody>
          </table>
        </td>
      </tr>

      <!-- Totals -->
      <tr>
        <td style=""padding:0 40px 28px;"">
          <table width=""300"" cellpadding=""0"" cellspacing=""0"" align=""right"" style=""background:#f9fafb;border-radius:8px;overflow:hidden;"">
            <tr>
              <td style=""padding:12px 20px;font-size:14px;color:#6b7280;"">Subtotal</td>
              <td style=""padding:12px 20px;font-size:14px;color:#374151;text-align:right;font-weight:600;"">{FormatCurrency(q.SubTotal)}</td>
            </tr>
            <tr>
              <td style=""padding:8px 20px;font-size:14px;color:#6b7280;border-top:1px solid #e5e7eb;"">Tax ({FormatCurrency(q.TaxRate)}%)</td>
              <td style=""padding:8px 20px;font-size:14px;color:#374151;text-align:right;font-weight:600;border-top:1px solid #e5e7eb;"">{FormatCurrency(q.TaxAmount)}</td>
            </tr>
            <tr>
              <td style=""padding:14px 20px;font-size:18px;font-weight:700;color:#1e40af;border-top:2px solid #1e40af;"">Total</td>
              <td style=""padding:14px 20px;font-size:18px;font-weight:700;color:#1e40af;text-align:right;border-top:2px solid #1e40af;"">{FormatCurrency(q.TotalAmount)}</td>
            </tr>
          </table>
        </td>
      </tr>

      <!-- Notes & Terms -->
      {(string.IsNullOrEmpty(q.Notes) && string.IsNullOrEmpty(q.TermsAndConditions) ? "" : $@"<tr>
        <td style=""padding:0 40px 28px;"">
          {(string.IsNullOrEmpty(q.Notes) ? "" : $@"<div style=""background:#fffbeb;border-left:4px solid #f59e0b;border-radius:0 8px 8px 0;padding:16px 20px;margin-bottom:12px;"">
            <p style=""margin:0 0 4px;font-size:12px;font-weight:600;color:#92400e;text-transform:uppercase;letter-spacing:0.5px;"">Notes</p>
            <p style=""margin:0;font-size:14px;color:#78350f;line-height:1.5;"">{Encode(q.Notes)}</p>
          </div>")}
          {(string.IsNullOrEmpty(q.TermsAndConditions) ? "" : $@"<div style=""background:#f0fdf4;border-left:4px solid #22c55e;border-radius:0 8px 8px 0;padding:16px 20px;"">
            <p style=""margin:0 0 4px;font-size:12px;font-weight:600;color:#166534;text-transform:uppercase;letter-spacing:0.5px;"">Terms &amp; Conditions</p>
            <p style=""margin:0;font-size:14px;color:#15803d;line-height:1.5;"">{Encode(q.TermsAndConditions)}</p>
          </div>")}
        </td>
      </tr>")}

      <!-- Footer -->
      <tr>
        <td style=""background:#f9fafb;padding:24px 40px;border-top:1px solid #e5e7eb;"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
            <tr>
              <td style=""text-align:center;"">
                <p style=""margin:0;font-size:13px;color:#6b7280;"">Thank you for your business!</p>
                <p style=""margin:8px 0 0;font-size:12px;color:#9ca3af;"">This quotation was generated by MaintenancePro &copy; {DateTime.UtcNow.Year}</p>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </td></tr>
</table>
</body>
</html>");

        return sb.ToString();
    }
}
