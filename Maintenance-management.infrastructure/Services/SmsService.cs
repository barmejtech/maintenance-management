using Maintenance_management.application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Maintenance_management.infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmsService> _logger;

    public SmsService(IConfiguration config, ILogger<SmsService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var smsSettings = _config.GetSection("SmsSettings");
            var accountSid = smsSettings["AccountSid"];
            var authToken = smsSettings["AuthToken"];
            var fromNumber = smsSettings["FromNumber"];

            if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken) || string.IsNullOrWhiteSpace(fromNumber))
            {
                _logger.LogWarning("SMS settings are not configured. Skipping SMS to {PhoneNumber}.", phoneNumber);
                return;
            }

            TwilioClient.Init(accountSid, authToken);

            await MessageResource.CreateAsync(
                to: new PhoneNumber(phoneNumber),
                from: new PhoneNumber(fromNumber),
                body: message);

            _logger.LogInformation("SMS sent successfully to {PhoneNumber}.", phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}.", phoneNumber);
        }
    }
}
