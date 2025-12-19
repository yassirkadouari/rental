using CarRentalApp.Data;
using CarRentalApp.Desktop.ViewModels;
using CarRentalApp.Desktop.Views;
using CarRentalApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace CarRentalApp.Desktop;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    public App()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginWindow>();
        
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
        
        services.AddSingleton<QrCodeService>();
        services.AddSingleton<PdfService>();
        services.AddSingleton<EmailService>();
        
        services.AddTransient<UsersViewModel>();
        services.AddTransient<VehiclesViewModel>();
        services.AddTransient<RentalsViewModel>();

        return services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var loginWindow = Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }
}

