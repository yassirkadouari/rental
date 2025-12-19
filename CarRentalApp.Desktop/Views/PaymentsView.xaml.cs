using CarRentalApp.Data;
using System.Linq;
using System.Windows;

namespace CarRentalApp.Desktop.Views;

public partial class PaymentsView : Window
{
    private readonly ApplicationDbContext _context;

    public PaymentsView()
    {
        InitializeComponent();
        _context = new ApplicationDbContextFactory().CreateDbContext(new string[0]);
        LoadData();
    }

    private void LoadData()
    {
        var payments = _context.Payments.ToList();
        PaymentsGrid.ItemsSource = payments;
        TxtTotal.Text = payments.Sum(p => p.Amount).ToString("C");
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        LoadData();
    }
}
