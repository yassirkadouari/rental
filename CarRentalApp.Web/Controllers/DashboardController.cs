using Microsoft.AspNetCore.Mvc;
using CarRentalApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Web.Controllers;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var totalVehicles = await _context.Vehicles.CountAsync();
        var activeRentals = await _context.Bookings.CountAsync(b => b.Status != Core.Enums.BookingStatus.Cancelled && b.EndDate >= DateTime.UtcNow);
        var totalRevenue = await _context.Bookings.SumAsync(b => (decimal?)b.TotalPrice) ?? 0m;

        ViewData["TotalVehicles"] = totalVehicles;
        ViewData["ActiveRentals"] = activeRentals;
        ViewData["TotalRevenue"] = totalRevenue;
        return View();
    }

    // API for charts
    public async Task<IActionResult> Stats()
    {
        var byMonth = await _context.Bookings
            .GroupBy(b => new { Year = b.StartDate.Year, Month = b.StartDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Revenue = g.Sum(x => x.TotalPrice) })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();
        return Json(byMonth);
    }
}
