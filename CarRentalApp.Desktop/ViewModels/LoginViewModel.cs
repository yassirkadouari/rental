using CarRentalApp.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CarRentalApp.Desktop.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApplicationDbContext _context;

    public LoginViewModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [ObservableProperty]
    private string _email = "admin@carrental.com"; // Default for testing

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public Action? OnLoginSuccess;

    [RelayCommand]
    private async Task Login(object? parameter)
    {
        ErrorMessage = string.Empty;
        
        var passwordBox = parameter as PasswordBox;
        var password = passwordBox?.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Please enter email and password.";
            return;
        }

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            
            // In a real app, verify hash. Here we compare plain text as seeded.
            if (user != null && user.PasswordHash == password)
            {
                if (user.Role == Core.Enums.UserRole.Client)
                {
                    ErrorMessage = "Access denied. Admins only.";
                    return;
                }

                OnLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = "Invalid email or password.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Database error: {ex.Message}";
        }
    }
}
