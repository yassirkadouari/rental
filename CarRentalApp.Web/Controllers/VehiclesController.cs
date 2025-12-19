using CarRentalApp.Data;
using CarRentalApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Web.Controllers;

public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ExcelService _excelService;

    public VehiclesController(ApplicationDbContext context, ExcelService excelService)
    {
        _context = context;
        _excelService = excelService;
    }

    public async Task<IActionResult> Index()
    {
        var vehicles = await _context.Vehicles
            .Include(v => v.VehicleType)
            .Where(v => v.Status == Core.Enums.VehicleStatus.Available)
            .ToListAsync();
            
        return View(vehicles);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleType)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (vehicle == null) return NotFound();

        return View(vehicle);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var vehicles = await _context.Vehicles.Include(v => v.VehicleType).ToListAsync();
        var bytes = _excelService.ExportToExcel(vehicles, "Vehicles");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "vehicles.xlsx");
    }
}
