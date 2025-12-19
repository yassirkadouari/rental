using CarRentalApp.Core.Entities;
using CarRentalApp.Core.Enums;
using System;
using System.Linq;

namespace CarRentalApp.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Check if DB exists (migrations should have created it)
        
        if (context.Users.Any())
        {
            return;   // DB has been seeded
        }

        var types = new VehicleType[]
        {
            new VehicleType{Name="Compact", Description="Small and economical"},
            new VehicleType{Name="Sedan", Description="Standard comfort"},
            new VehicleType{Name="SUV", Description="Spacious and powerful"}
        };
        
        context.VehicleTypes.AddRange(types);
        context.SaveChanges();

        var users = new User[]
        {
            new User{FirstName="Admin", LastName="System", Email="admin@carrental.com", PasswordHash="Admin@123", Role=UserRole.Admin, PhoneNumber="000000000"},
            new User{FirstName="Agent", LastName="Smith", Email="agent@carrental.com", PasswordHash="Agent@123", Role=UserRole.Employee, PhoneNumber="111111111"},
            new User{FirstName="John", LastName="Doe", Email="john@client.com", PasswordHash="Client@123", Role=UserRole.Client, PhoneNumber="222222222"}
        };
        
        context.Users.AddRange(users);
        context.SaveChanges();

        var vehicles = new Vehicle[]
        {
            // Compact Cars
            new Vehicle{Brand="Toyota", Model="Yaris", Year=2023, LicensePlate="123-TU-2023", DailyRate=45, Status=VehicleStatus.Available, VehicleTypeId=types[0].Id, ImageUrl="https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?w=800"},
            new Vehicle{Brand="Honda", Model="Civic", Year=2024, LicensePlate="124-TU-2024", DailyRate=50, Status=VehicleStatus.Available, VehicleTypeId=types[0].Id, ImageUrl="https://images.unsplash.com/photo-1590362891991-f776e747a588?w=800"},
            new Vehicle{Brand="Volkswagen", Model="Golf", Year=2023, LicensePlate="125-TU-2023", DailyRate=48, Status=VehicleStatus.Available, VehicleTypeId=types[0].Id, ImageUrl="https://images.unsplash.com/photo-1622353219448-46a009f0d44f?w=800"},
            new Vehicle{Brand="Ford", Model="Focus", Year=2024, LicensePlate="126-TU-2024", DailyRate=47, Status=VehicleStatus.Available, VehicleTypeId=types[0].Id, ImageUrl="https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800"},
            new Vehicle{Brand="Renault", Model="Clio", Year=2023, LicensePlate="127-TU-2023", DailyRate=42, Status=VehicleStatus.Available, VehicleTypeId=types[0].Id, ImageUrl="https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=800"},
            
            // Sedans
            new Vehicle{Brand="BMW", Model="Series 3", Year=2024, LicensePlate="456-TU-2024", DailyRate=120, Status=VehicleStatus.Available, VehicleTypeId=types[1].Id, ImageUrl="https://images.unsplash.com/photo-1555215695-3004980ad54e?w=800"},
            new Vehicle{Brand="Mercedes", Model="C-Class", Year=2024, LicensePlate="457-TU-2024", DailyRate=130, Status=VehicleStatus.Available, VehicleTypeId=types[1].Id, ImageUrl="https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=800"},
            new Vehicle{Brand="Audi", Model="A4", Year=2023, LicensePlate="458-TU-2023", DailyRate=125, Status=VehicleStatus.Available, VehicleTypeId=types[1].Id, ImageUrl="https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=800"},
            new Vehicle{Brand="Tesla", Model="Model 3", Year=2024, LicensePlate="459-TU-2024", DailyRate=150, Status=VehicleStatus.Available, VehicleTypeId=types[1].Id, ImageUrl="https://images.unsplash.com/photo-1560958089-b8a1929cea89?w=800"},
            new Vehicle{Brand="Lexus", Model="IS 300", Year=2023, LicensePlate="460-TU-2023", DailyRate=135, Status=VehicleStatus.Available, VehicleTypeId=types[1].Id, ImageUrl="https://images.unsplash.com/photo-1555652408-8e1d3a5e3d6f?w=800"},
            new Vehicle{Brand="Jaguar", Model="XE", Year=2024, LicensePlate="461-TU-2024", DailyRate=140, Status=VehicleStatus.Available, VehicleTypeId=types[1].Id, ImageUrl="https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=800"},
            new Vehicle{Brand="Volvo", Model="S60", Year=2023, LicensePlate="462-TU-2023", DailyRate=115, Status=VehicleStatus.Available, VehicleTypeId=types[1].Id, ImageUrl="https://images.unsplash.com/photo-1617814076367-b759c7d7e738?w=800"},
            new Vehicle{Brand="Alfa Romeo", Model="Giulia", Year=2024, LicensePlate="463-TU-2024", DailyRate=145, Status=VehicleStatus.Available, VehicleTypeId=types[1].Id, ImageUrl="https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800"},
            
            // SUVs & Luxury
            new Vehicle{Brand="Range Rover", Model="Evoque", Year=2024, LicensePlate="789-TU-2024", DailyRate=200, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800"},
            new Vehicle{Brand="Porsche", Model="Cayenne", Year=2024, LicensePlate="790-TU-2024", DailyRate=250, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1580273916550-e323be2ae537?w=800"},
            new Vehicle{Brand="BMW", Model="X5", Year=2024, LicensePlate="791-TU-2024", DailyRate=180, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1617469767053-d3b523a0b982?w=800"},
            new Vehicle{Brand="Mercedes", Model="GLE", Year=2024, LicensePlate="792-TU-2024", DailyRate=190, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1606220588913-b3aacb4d2f46?w=800"},
            new Vehicle{Brand="Audi", Model="Q7", Year=2023, LicensePlate="793-TU-2023", DailyRate=175, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1603584173870-7f23fdae1b7a?w=800"},
            new Vehicle{Brand="Lexus", Model="RX 350", Year=2024, LicensePlate="794-TU-2024", DailyRate=170, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800"},
            new Vehicle{Brand="Tesla", Model="Model X", Year=2024, LicensePlate="795-TU-2024", DailyRate=220, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1617788138017-80ad40651399?w=800"},
            new Vehicle{Brand="Jeep", Model="Grand Cherokee", Year=2023, LicensePlate="796-TU-2023", DailyRate=160, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?w=800"},
            new Vehicle{Brand="Volvo", Model="XC90", Year=2024, LicensePlate="797-TU-2024", DailyRate=185, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=800"},
            new Vehicle{Brand="Land Rover", Model="Discovery", Year=2023, LicensePlate="798-TU-2023", DailyRate=195, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1609521263047-f8f205293f24?w=800"},
            new Vehicle{Brand="Maserati", Model="Levante", Year=2024, LicensePlate="799-TU-2024", DailyRate=280, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=800"},
            new Vehicle{Brand="Bentley", Model="Bentayga", Year=2024, LicensePlate="800-TU-2024", DailyRate=350, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1563720223185-11003d516935?w=800"},
            new Vehicle{Brand="Lamborghini", Model="Urus", Year=2024, LicensePlate="801-TU-2024", DailyRate=500, Status=VehicleStatus.Available, VehicleTypeId=types[2].Id, ImageUrl="https://images.unsplash.com/photo-1544829099-b9a0c07fad1a?w=800"}
        };
        
        context.Vehicles.AddRange(vehicles);
        context.SaveChanges();
    }
}
