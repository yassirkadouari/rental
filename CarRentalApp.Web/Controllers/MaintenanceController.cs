using CarRentalApp.Core.Entities;
using CarRentalApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Web.Controllers;

public class MaintenanceController : Controller
{
    private readonly ApplicationDbContext _context;

    public MaintenanceController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _context.Maintenances.Include(m => m.Vehicle).OrderByDescending(m => m.Date).ToListAsync();
        return View(list);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Maintenance model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Maintenances.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Complete(int id)
    {
        var m = await _context.Maintenances.FindAsync(id);
        if (m == null) return NotFound();
        m.IsCompleted = true;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
