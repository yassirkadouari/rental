using CarRentalApp.Core.Entities;
using CarRentalApp.Core.Enums;
using CarRentalApp.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CarRentalApp.Desktop.ViewModels;

public partial class VehiclesViewModel : ObservableObject
{
    private readonly ApplicationDbContext _context;

    public VehiclesViewModel(ApplicationDbContext context)
    {
        _context = context;
        LoadDataCommand.Execute(null);
    }

    [ObservableProperty]
    private ObservableCollection<Vehicle> _vehicles = new();

    [ObservableProperty]
    private ObservableCollection<VehicleType> _vehicleTypes = new();

    [ObservableProperty]
    private Vehicle _selectedVehicle;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    // For Editing
    [ObservableProperty]
    private Vehicle _editingVehicle = new();

    [ObservableProperty]
    private bool _isEditing = false;
    
    // UI selections
    [ObservableProperty]
    private VehicleType _selectedVehicleType;
    
    [ObservableProperty]
    private VehicleStatus _selectedStatus;

    public VehicleStatus[] Statuses => (VehicleStatus[])Enum.GetValues(typeof(VehicleStatus));

    [ObservableProperty]
    private string _searchText = string.Empty;

    partial void OnSearchTextChanged(string value)
    {
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        try
        {
            var types = await _context.VehicleTypes.ToListAsync();
            VehicleTypes = new ObservableCollection<VehicleType>(types);

            var query = _context.Vehicles.Include(v => v.VehicleType).AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(v => v.Brand.Contains(SearchText) || v.Model.Contains(SearchText) || v.LicensePlate.Contains(SearchText));
            }

            var list = await query.ToListAsync();
            Vehicles = new ObservableCollection<Vehicle>(list);
            StatusMessage = "Data loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading data: {ex.Message}";
        }
    }

    partial void OnSelectedVehicleChanged(Vehicle value)
    {
        if (value != null)
        {
            // Clone for editing to avoid direct binding to context entity before save
            EditingVehicle = new Vehicle
            {
                Id = value.Id,
                Brand = value.Brand,
                Model = value.Model,
                Year = value.Year,
                LicensePlate = value.LicensePlate,
                DailyRate = value.DailyRate,
                Status = value.Status,
                VehicleTypeId = value.VehicleTypeId,
                ImageUrl = value.ImageUrl
            };
            IsEditing = true;
            SelectedVehicleType = null; // Bind issue helper if needed
        }
    }

    [RelayCommand]
    private void StartNew()
    {
        EditingVehicle = new Vehicle { Status = VehicleStatus.Available, Year = DateTime.Now.Year };
        SelectedVehicle = null;
        IsEditing = true;
    }

    [RelayCommand]
    private void UploadImage()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            EditingVehicle.ImageUrl = openFileDialog.FileName;
            OnPropertyChanged(nameof(EditingVehicle));
        }
    }

    [RelayCommand]
    private async Task Import()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
        if (openFileDialog.ShowDialog() == true)
        {
             try
             {
                 var service = new CarRentalApp.Services.ExcelService();
                 var list = service.ImportVehicles(openFileDialog.FileName);
                 
                 foreach(var v in list)
                 {
                     _context.Vehicles.Add(v);
                 }
                 await _context.SaveChangesAsync();
                 await LoadData();
                 StatusMessage = $"Imported {list.Count} vehicles.";
             }
             catch (Exception ex)
             {
                 StatusMessage = $"Import Error: {ex.Message}";
             }
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(EditingVehicle.Brand) || string.IsNullOrWhiteSpace(EditingVehicle.Model))
        {
            StatusMessage = "Brand and Model are required.";
            return;
        }

        try
        {
            if (EditingVehicle.Id == 0)
            {
                _context.Vehicles.Add(EditingVehicle);
            }
            else
            {
                var existing = await _context.Vehicles.FindAsync(EditingVehicle.Id);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(EditingVehicle);
                }
            }

            await _context.SaveChangesAsync();
            await LoadData();
            StatusMessage = "Saved successfully.";
            IsEditing = false;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task Delete(Vehicle vehicle)
    {
        if (vehicle == null) return;
        if (MessageBox.Show($"Delete {vehicle.Brand} {vehicle.Model}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                Vehicles.Remove(vehicle);
                StatusMessage = "Deleted.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting: {ex.Message}";
            }
        }
    }
}
