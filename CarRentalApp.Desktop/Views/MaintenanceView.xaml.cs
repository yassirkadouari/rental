using CarRentalApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace CarRentalApp.Desktop.Views;

public partial class MaintenanceView : Window
{
    private readonly ApplicationDbContext _context;

    public MaintenanceView()
    {
        InitializeComponent();
        _context = new ApplicationDbContextFactory().CreateDbContext(new string[0]);
        LoadData();
    }

    private void LoadData()
    {
        MaintenanceGrid.ItemsSource = _context.Maintenances
            .Include(m => m.Vehicle)
            .ToList();
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Add Maintenance dialog would open here", "Info");
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        LoadData();
    }
}
