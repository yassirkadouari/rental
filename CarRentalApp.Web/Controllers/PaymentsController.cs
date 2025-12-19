using CarRentalApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CarRentalApp.Web.Controllers;

public class PaymentsController : Controller
{
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;

    public PaymentsController(IConfiguration config, ApplicationDbContext context)
    {
        _config = config;
        _context = context;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        
        var payments = await _context.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b.Vehicle)
            .Where(p => p.Booking.UserId == userId)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

        return View(payments);
    }

    // Simple PayPal checkout scaffold - requires client config in appsettings
    public IActionResult Checkout(decimal amount = 0)
    {
        ViewData["PayPalClientId"] = _config["PayPal:ClientId"] ?? string.Empty;
        ViewData["Amount"] = amount;
        return View();
    }

    // Called by PayPal after payment success (in real integration validate server-side)
    public IActionResult Success(string orderId)
    {
        ViewData["OrderId"] = orderId;
        return View();
    }

    public IActionResult Cancel()
    {
        return View();
    }
}
