using System;

namespace CarRentalApp.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; }
    
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string PaymentMethod { get; set; } // "Card", "Cash", "Transfer"
    public string TransactionId { get; set; }
}
