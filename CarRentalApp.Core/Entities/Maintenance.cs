using System;

namespace CarRentalApp.Core.Entities;

public class Maintenance
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }
    
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public decimal Cost { get; set; }
    public bool IsCompleted { get; set; }
}
