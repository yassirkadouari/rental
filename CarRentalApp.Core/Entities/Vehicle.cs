using CarRentalApp.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApp.Core.Entities;

public class Vehicle : BaseEntity
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public string? ImageUrl { get; set; }
    public VehicleStatus Status { get; set; }
    
    public int VehicleTypeId { get; set; }
    [ForeignKey("VehicleTypeId")]
    public VehicleType VehicleType { get; set; } = null!;
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
