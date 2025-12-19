using CarRentalApp.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApp.Core.Entities;

public class Booking : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    
    public DateTime? ActualReturnDate { get; set; }
    public string? ReturnNotes { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    public int VehicleId { get; set; }
    [ForeignKey("VehicleId")]
    public Vehicle Vehicle { get; set; } = null!;
}
