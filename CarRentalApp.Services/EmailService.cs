using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;

namespace CarRentalApp.Services;

public class SmtpSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public bool EnableSsl { get; set; } = true;
    public string FromName { get; set; } = "Car Rental Application";
    public string FromAddress { get; set; } = "";
}

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IConfiguration configuration)
    {
        // Read from environment variables first, then fall back to configuration (appsettings.json)
        string env(string key) => Environment.GetEnvironmentVariable(key);

        var hostEnv = env("SMTP_HOST");
        var portEnv = env("SMTP_PORT");
        var usernameEnv = env("SMTP_USERNAME");
        var passwordEnv = env("SMTP_PASSWORD");
        var enableSslEnv = env("SMTP_ENABLESSL");
        var fromAddressEnv = env("SMTP_FROMADDRESS");
        var fromNameEnv = env("SMTP_FROMNAME");

        // Read config section
        var section = configuration.GetSection("Smtp");

        _smtp = new SmtpSettings
        {
            Host = !string.IsNullOrWhiteSpace(hostEnv) ? hostEnv : section.GetValue<string>("Host", "smtp.gmail.com"),
            Port = int.TryParse(portEnv, out var p) ? p : section.GetValue<int>("Port", 587),
            Username = !string.IsNullOrWhiteSpace(usernameEnv) ? usernameEnv : section.GetValue<string>("Username", ""),
            Password = !string.IsNullOrWhiteSpace(passwordEnv) ? passwordEnv : section.GetValue<string>("Password", ""),
            EnableSsl = bool.TryParse(enableSslEnv, out var sVal) ? sVal : section.GetValue<bool>("EnableSsl", true),
            FromAddress = !string.IsNullOrWhiteSpace(fromAddressEnv) ? fromAddressEnv : section.GetValue<string>("FromAddress", ""),
            FromName = !string.IsNullOrWhiteSpace(fromNameEnv) ? fromNameEnv : section.GetValue<string>("FromName", "Car Rental Application")
        };
    }

    public async Task SendBookingConfirmationAsync(string toEmail, int bookingId, byte[] attachmentPdf)
    {
        try
        {
            using (var message = new MailMessage())
            {
                var fromAddress = string.IsNullOrWhiteSpace(_smtp.FromAddress) ? _smtp.Username : _smtp.FromAddress;
                message.From = new MailAddress(fromAddress, _smtp.FromName ?? "Car Rental Application");
                message.To.Add(new MailAddress(toEmail));
                message.Subject = $"Booking Confirmation - #{bookingId}";
                message.Body = $"Dear Customer,\n\nYour booking #{bookingId} has been confirmed. Please find the attached contract.\n\nThank you for choosing us.";

                if (attachmentPdf != null && attachmentPdf.Length > 0)
                {
                    message.Attachments.Add(new Attachment(new MemoryStream(attachmentPdf), $"Contract_{bookingId}.pdf", "application/pdf"));
                }

                if (string.IsNullOrWhiteSpace(_smtp.Username) || _smtp.Username == "your-email@gmail.com")
                {
                    // Simulate send when SMTP not configured
                    System.Diagnostics.Debug.WriteLine($"[EmailService] SMTP not configured. Simulated send to {toEmail}");
                    return;
                }

                using (var client = new SmtpClient(_smtp.Host, _smtp.Port))
                {
                    client.EnableSsl = _smtp.EnableSsl;
                    client.Credentials = new NetworkCredential(_smtp.Username, _smtp.Password);
                    await client.SendMailAsync(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
    }
}
