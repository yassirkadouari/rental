using System.Threading.Tasks;

namespace CarRentalApp.Services;

public interface IEmailService
{
    Task SendBookingConfirmationAsync(string toEmail, int bookingId, byte[] attachmentPdf);
}
