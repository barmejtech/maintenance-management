using Maintenance_management.application.DTOs.Quotation;

namespace Maintenance_management.application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    Task SendQuotationEmailAsync(string toEmail, QuotationDto quotation);
}
