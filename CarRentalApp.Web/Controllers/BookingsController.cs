using CarRentalApp.Core.Entities;
using CarRentalApp.Core.Enums;
using CarRentalApp.Data;
using CarRentalApp.Web.Models;
using CarRentalApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CarRentalApp.Web.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly PdfService _pdfService;

    public BookingsController(ApplicationDbContext context, IEmailService emailService, PdfService pdfService)
    {
        _context = context;
        _emailService = emailService;
        _pdfService = pdfService;
    }

    // GET: User's Bookings
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var bookings = await _context.Bookings
            .Include(b => b.Vehicle)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
            
        return View(bookings);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int vehicleId)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        if (vehicle == null) return NotFound();

        var vm = new CreateBookingViewModel
        {
            VehicleId = vehicle.Id,
            VehicleBrand = vehicle.Brand,
            VehicleModel = vehicle.Model,
            DailyRate = vehicle.DailyRate,
            ImageUrl = vehicle.ImageUrl
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(CreateBookingViewModel model)
    {
        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError("", "End date must be after start date.");
            return View("Create", model);
        }

        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        
        // Check if user exists (important after database reset)
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        var days = (model.EndDate - model.StartDate).Days;
        
        var total = days * model.DailyRate;

        var booking = new Booking
        {
            UserId = userId,
            VehicleId = model.VehicleId,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            TotalPrice = total,
            Status = BookingStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Create Payment Record
        var payment = new Core.Entities.Payment
        {
            BookingId = booking.Id,
            Amount = total,
            Date = DateTime.UtcNow,
            PaymentMethod = model.PaymentMethod,
            TransactionId = $"TXN-{booking.Id}-{DateTime.UtcNow.Ticks}"
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Send confirmation email with PDF
        _ = SendConfirmationEmailAsync(booking, payment);

        TempData["SuccessMessage"] = $"Booking confirmed! Payment of ${total} via {model.PaymentMethod} recorded. Check your email for confirmation.";
        return RedirectToAction(nameof(Index));
    }

    private async Task SendConfirmationEmailAsync(Core.Entities.Booking booking, Core.Entities.Payment payment)
    {
        try
        {
            var user = await _context.Users.FindAsync(booking.UserId);
            var vehicle = await _context.Vehicles.FindAsync(booking.VehicleId);
            if (user != null && vehicle != null)
            {
                var pdf = _pdfService.GenerateContract(booking, user, vehicle);
                
                // Enhanced email body with payment details
                var emailBody = $@"Dear {user.FirstName} {user.LastName},

Your booking has been confirmed!

Booking Details:
- Booking ID: #{booking.Id}
- Vehicle: {vehicle.Brand} {vehicle.Model}
- Pick-up Date: {booking.StartDate:dd/MM/yyyy}
- Return Date: {booking.EndDate:dd/MM/yyyy}
- Total Price: ${booking.TotalPrice}

Payment Confirmation:
- Payment Method: {payment.PaymentMethod}
- Transaction ID: {payment.TransactionId}
- Amount Paid: ${payment.Amount}
- Payment Date: {payment.Date:dd/MM/yyyy HH:mm}

Please find your contract PDF attached to this email.

Thank you for choosing RentACar!

Best regards,
The RentACar Team";

                // Note: EmailService needs to be updated to accept custom body
                await _emailService.SendBookingConfirmationAsync(user.Email, booking.Id, pdf);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending confirmation: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var bookings = await _context.Bookings
            .Include(b => b.Vehicle)
            .Include(b => b.User)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Vehicle,StartDate,EndDate,TotalPrice,Status");
        foreach (var b in bookings)
        {
            sb.AppendLine($"{b.Id},\"{b.Vehicle.Brand} {b.Vehicle.Model}\",{b.StartDate:yyyy-MM-dd},{b.EndDate:yyyy-MM-dd},{b.TotalPrice},{b.Status}");
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", "bookings.csv");
    }

    [HttpGet]
    public async Task<IActionResult> DownloadPdf(int id)
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var booking = await _context.Bookings
            .Include(b => b.Vehicle)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null) return NotFound();

        var pdf = _pdfService.GenerateContract(booking, booking.User, booking.Vehicle);
        return File(pdf, "application/pdf", $"Contract_{booking.Id}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> ViewQrCode(int id)
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var booking = await _context.Bookings
            .Include(b => b.Vehicle)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null) return NotFound();

        var qrService = new QrCodeService();
        string qrContent = $"Booking #{booking.Id} | User: {booking.User.Email} | Vehicle: {booking.Vehicle.Brand} {booking.Vehicle.Model} | Valid: {booking.EndDate:yyyy-MM-dd}";
        var qrBytes = qrService.GenerateQrCode(qrContent);

        return File(qrBytes, "image/png");
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var bookings = await _context.Bookings
            .Include(b => b.Vehicle)
            .Where(b => b.UserId == userId)
            .Select(b => new {
                b.Id,
                Vehicle = b.Vehicle.Brand + " " + b.Vehicle.Model,
                b.StartDate,
                b.EndDate,
                b.TotalPrice,
                Status = b.Status.ToString()
            })
            .ToListAsync();

        var excelService = new ExcelService();
        var bytes = excelService.ExportToExcel(bookings, "My Bookings");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "my_bookings.xlsx");
    }
}
