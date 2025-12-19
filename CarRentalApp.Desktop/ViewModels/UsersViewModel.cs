using CarRentalApp.Core.Entities;
using CarRentalApp.Desktop.Views;
using CarRentalApp.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CarRentalApp.Core.Enums;
using System;
using System.Linq;

namespace CarRentalApp.Desktop.ViewModels;

public partial class UsersViewModel : ObservableObject
{
    private readonly ApplicationDbContext _context;

    public UsersViewModel(ApplicationDbContext context)
    {
        _context = context;
        LoadUsersCommand.Execute(null);
    }

    [ObservableProperty]
    private ObservableCollection<User> _users = new();

    [ObservableProperty]
    private User? _selectedUser;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [RelayCommand]
    private async Task LoadUsers()
    {
        try 
        {
            var list = await _context.Users.ToListAsync();
            Users = new ObservableCollection<User>(list);
            StatusMessage = $"Loaded {Users.Count} users.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }
    
    [RelayCommand]
    private async Task SaveUser(User user)
    {
        // Simple save logic (usually involves checking if new or existing)
        try
        {
             if (user.Id == 0) _context.Users.Add(user);
             else _context.Users.Update(user);
             
             await _context.SaveChangesAsync();
             await LoadUsers();
        }
        catch (Exception ex)
        {
             StatusMessage = $"Error saving: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteUser(User? user)
    {
        if (user == null) return;
        if (MessageBox.Show($"Are you sure you want to delete {user.Email}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                Users.Remove(user);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting: {ex.Message}";
            }
        }
    }
}
