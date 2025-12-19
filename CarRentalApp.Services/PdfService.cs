using CarRentalApp.Core.Entities;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.IO;

namespace CarRentalApp.Services;

public class PdfService
{
    private readonly QrCodeService _qrCodeService;

    public PdfService()
    {
        _qrCodeService = new QrCodeService();
    }

    public byte[] GenerateContract(Booking booking, User user, Vehicle vehicle)
    {
        using (var document = new PdfDocument())
        {
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var fontHeader = new XFont("Arial", 20, XFontStyle.Bold);
            var fontNormal = new XFont("Arial", 12, XFontStyle.Regular);
            var fontSmall = new XFont("Arial", 10, XFontStyle.Italic);

            // Title
            gfx.DrawString("RENTAL AGREEMENT", fontHeader, XBrushes.Black, new XRect(0, 40, page.Width, page.Height), XStringFormats.TopCenter);

            // Contract Details
            int y = 100;
            gfx.DrawString($"Booking ID: #{booking.Id}", fontNormal, XBrushes.Black, 50, y); y += 20;
            gfx.DrawString($"Date: {DateTime.Now.ToShortDateString()}", fontNormal, XBrushes.Black, 50, y); y += 40;

            // Tenant
            gfx.DrawString("TENANT", new XFont("Arial", 14, XFontStyle.Bold), XBrushes.Black, 50, y); y += 25;
            gfx.DrawString($"Name: {user.FirstName} {user.LastName}", fontNormal, XBrushes.Black, 50, y); y += 20;
            gfx.DrawString($"Email: {user.Email}", fontNormal, XBrushes.Black, 50, y); y += 20;
            gfx.DrawString($"Phone: {user.PhoneNumber}", fontNormal, XBrushes.Black, 50, y); y += 40;

            // Vehicle
            gfx.DrawString("VEHICLE", new XFont("Arial", 14, XFontStyle.Bold), XBrushes.Black, 50, y); y += 25;
            gfx.DrawString($"Car: {vehicle.Brand} {vehicle.Model} ({vehicle.Year})", fontNormal, XBrushes.Black, 50, y); y += 20;
            gfx.DrawString($"Plate: {vehicle.LicensePlate}", fontNormal, XBrushes.Black, 50, y); y += 40;

            // Rental Details
            gfx.DrawString("RENTAL PERIOD", new XFont("Arial", 14, XFontStyle.Bold), XBrushes.Black, 50, y); y += 25;
            gfx.DrawString($"From: {booking.StartDate.ToShortDateString()}", fontNormal, XBrushes.Black, 50, y); y += 20;
            gfx.DrawString($"To: {booking.EndDate.ToShortDateString()}", fontNormal, XBrushes.Black, 50, y); y += 20;
            gfx.DrawString($"Total Price: ${booking.TotalPrice}", new XFont("Arial", 14, XFontStyle.Bold), XBrushes.Blue, 50, y); y += 60;

            // Signatures
            gfx.DrawString("Signature: __________________________", fontNormal, XBrushes.Black, 50, y); 
            
            // QR Code at bottom
            string qrContent = $"Contract #{booking.Id} | User: {user.Id} | Vehicle: {vehicle.Id} | Valid: {booking.EndDate.ToShortDateString()}";
            var qrBytes = _qrCodeService.GenerateQrCode(qrContent);
            using (var ms = new MemoryStream(qrBytes))
            {
                var image = XImage.FromStream(() => ms);
                gfx.DrawImage(image, 400, y - 50, 100, 100);
            }

            using (var stream = new MemoryStream())
            {
                document.Save(stream);
                try
                {
                    // ensure folder exists and write a copy for inspection
                    var exportsDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Exports");
                    Directory.CreateDirectory(exportsDir);
                    var filePath = Path.Combine(exportsDir, $"Contract_{booking.Id}.pdf");
                    File.WriteAllBytes(filePath, stream.ToArray());
                    Console.WriteLine($"[PdfService] Contract saved to: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PdfService] Failed to save contract to disk: {ex.Message}");
                }

                return stream.ToArray();
            }
        }
    }
}
