using CarRentalApp.Data;
using CarRentalApp.Desktop.Views;
using CarRentalApp.Services;
using System.Linq;
using System.Windows;

namespace CarRentalApp.Desktop;

public partial class MainWindow : Window
{
    private readonly ApplicationDbContext _context;
    // We would normally use Dependency Injection or a ViewModel here
    
    public MainWindow()
    {
        InitializeComponent();
        _context = new ApplicationDbContextFactory().CreateDbContext(new string[0]);
        LoadStats();
    }

    private void LoadStats()
    {
        TxtTotalVehicles.Text = _context.Vehicles.Count().ToString();
        TxtActiveRentals.Text = _context.Bookings.Where(b => b.EndDate >= System.DateTime.Now).Count().ToString();
        TxtMaintenance.Text = _context.Maintenances.Where(m => !m.IsCompleted).Count().ToString();
    }

    private void Vehicles_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new VehiclesView();
    }

    private void Rentals_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new RentalsView();
    }

    private void Users_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new UsersView();
    }
    
    private void Maintenance_Click(object sender, RoutedEventArgs e)
    {
        var view = new MaintenanceView();
        view.Show();
    }

    private void Payments_Click(object sender, RoutedEventArgs e)
    {
        var view = new PaymentsView();
        view.Show();
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        var service = new ExcelService();
        var vehicles = _context.Vehicles.ToList();
        var bytes = service.ExportToExcel(vehicles, "Vehicles");
        
        System.IO.File.WriteAllBytes("Vehicles_Export.xlsx", bytes);
        MessageBox.Show("Exported to Vehicles_Export.xlsx", "Success");
    }
}
