using CarRentalApp.Desktop.ViewModels;
using System.Windows;

namespace CarRentalApp.Desktop.Views;

public partial class LoginWindow : Window
{
    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        viewModel.OnLoginSuccess = () =>
        {
            var mainWindow = App.Current.Services.GetService(typeof(MainWindow)) as Window;
            mainWindow?.Show();
            this.Close();
        };
    }
}
