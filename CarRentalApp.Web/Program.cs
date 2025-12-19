using CarRentalApp.Data;
using CarRentalApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure File Logging
builder.Logging.AddFile("Logs/app-{Date}.txt");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure SMTP settings and register Email service
// Register email service (reads Smtp config via IConfiguration)
builder.Services.AddScoped<IEmailService, EmailService>();

// Register PdfService so it can be used by controllers
builder.Services.AddScoped<PdfService>();
// Excel export
builder.Services.AddScoped<ExcelService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();



using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
    // --- Test: generate a sample booking and PDF if no bookings exist ---
    try
    {
        // Don't interfere if data already present
        if (!context.Bookings.Any())
        {
            // create or get a test user
            var user = context.Users.FirstOrDefault();
            if (user == null)
            {
                user = new CarRentalApp.Core.Entities.User
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com",
                    PhoneNumber = "0000000000",
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(user);
                context.SaveChanges();
            }

            // create or get a vehicle
            var vehicle = context.Vehicles.FirstOrDefault();
            if (vehicle == null)
            {
                vehicle = new CarRentalApp.Core.Entities.Vehicle
                {
                    Brand = "TestBrand",
                    Model = "TestModel",
                    Year = 2020,
                    LicensePlate = "TEST-001",
                    DailyRate = 25m,
                    CreatedAt = DateTime.UtcNow,
                    VehicleTypeId = context.VehicleTypes.Select(vt => vt.Id).FirstOrDefault()
                };
                context.Vehicles.Add(vehicle);
                context.SaveChanges();
            }

            var booking = new CarRentalApp.Core.Entities.Booking
            {
                UserId = user.Id,
                VehicleId = vehicle.Id,
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(2),
                TotalPrice = vehicle.DailyRate * 2,
                Status = CarRentalApp.Core.Enums.BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            context.Bookings.Add(booking);
            context.SaveChanges();

            var pdfService = services.GetRequiredService<PdfService>();
            var pdf = pdfService.GenerateContract(booking, user, vehicle);
            Console.WriteLine($"[Startup Test] Generated test contract for booking #{booking.Id}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Startup Test] Failed to generate test booking/pdf: {ex.Message}");
    }
}

app.Run();
