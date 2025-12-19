using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Web.Models;

public class CreateBookingViewModel
{
    public int VehicleId { get; set; }
    public string VehicleBrand { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public string? ImageUrl { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(1);

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(3);

    public string PaymentMethod { get; set; } = "Card";

    public decimal TotalPrice => (EndDate - StartDate).Days * DailyRate;
}
