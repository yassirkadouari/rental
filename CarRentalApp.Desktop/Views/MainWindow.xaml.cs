using CarRentalApp.Desktop.ViewModels;
using System.Windows;

namespace CarRentalApp.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
