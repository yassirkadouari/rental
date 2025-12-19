using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CarRentalApp.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IServiceProvider _services;

    public MainViewModel(IServiceProvider services)
    {
        _services = services;
        // Default View or Welcome
    }

    [ObservableProperty]
    private object _currentView;
    
    [ObservableProperty]
    private string _welcomeMessage = "Welcome to Car Rental Administration";

    [RelayCommand]
    public void NavigateToUsers()
    {
        CurrentView = _services.GetRequiredService<UsersViewModel>();
    }

    [RelayCommand]
    public void NavigateToVehicles()
    {
        CurrentView = _services.GetRequiredService<VehiclesViewModel>();
    }
    
    [RelayCommand]
    public void NavigateToRentals()
    {
        CurrentView = _services.GetRequiredService<RentalsViewModel>();
    }
    
    [RelayCommand]
    public void NavigateToHome()
    {
        CurrentView = null; // Shows Welcome Message if handled in View
    }
}
