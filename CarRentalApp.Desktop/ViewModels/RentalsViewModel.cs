using CarRentalApp.Core.Entities;
using CarRentalApp.Core.Enums;
using CarRentalApp.Data;
using CarRentalApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace CarRentalApp.Desktop.ViewModels;

public partial class RentalsViewModel : ObservableObject
{
    private readonly ApplicationDbContext _context;
    private readonly PdfService _pdfService;
    private readonly IEmailService _emailService;

    public RentalsViewModel(ApplicationDbContext context, PdfService pdfService, IEmailService emailService)
    {
        _context = context;
        _pdfService = pdfService;
        _emailService = emailService;
        LoadRentalsCommand.Execute(null);
    }

    [ObservableProperty]
    private ObservableCollection<Booking> _bookings = new();

    [ObservableProperty]
    private Booking _selectedBooking;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [RelayCommand]
    private async Task LoadRentals()
    {
        try
        {
            var list = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Vehicle)
                .ToListAsync();
            
            Bookings = new ObservableCollection<Booking>(list);
            StatusMessage = $"Loaded {Bookings.Count} rentals.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ApproveBooking(Booking booking)
    {
        if (booking == null) return;
        
        try 
        {
            var vehicle = await _context.Vehicles.FindAsync(booking.VehicleId);
            if (vehicle == null) return;

            booking.Status = BookingStatus.Confirmed;
            vehicle.Status = VehicleStatus.Rented; // Mark vehicle as rented

            await _context.SaveChangesAsync();
            
            // Generate Contract
            StatusMessage = "Generatng Contract...";
            var pdfBytes = _pdfService.GenerateContract(booking, booking.User, booking.Vehicle);
            
            // Save to temp
            string path = Path.Combine(Path.GetTempPath(), $"Contract_Rental_{booking.Id}.pdf");
            File.WriteAllBytes(path, pdfBytes);
            
            // Send Email (Fire and forget)
            _ = _emailService.SendBookingConfirmationAsync(booking.User.Email, booking.Id, pdfBytes);
            
            StatusMessage = $"Approved. Contract saved to {path}";
            
            // Open PDF
            new Process { StartInfo = new ProcessStartInfo(path) { UseShellExecute = true } }.Start();
            
            await LoadRentals();
        } 
        catch (Exception ex)
        {
            StatusMessage = $"Error generating contract: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ReturnVehicle(Booking booking)
    {
        if (booking == null) return;
        try
        {
            var vehicle = await _context.Vehicles.FindAsync(booking.VehicleId);
            if (vehicle == null) return;

            booking.Status = BookingStatus.Completed;
            booking.ActualReturnDate = DateTime.Now;
            
            vehicle.Status = VehicleStatus.Available; // Release vehicle

            await _context.SaveChangesAsync();
            await LoadRentals();
            StatusMessage = "Vehicle Returned.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error returning: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CancelBooking(Booking booking)
    {
        if (booking == null) return;
        try 
        {
            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();
            await LoadRentals();
            StatusMessage = "Booking Cancelled.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }
}
