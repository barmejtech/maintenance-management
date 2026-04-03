namespace Maintenance_management.application.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
}
