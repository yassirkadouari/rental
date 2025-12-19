namespace CarRentalApp.Core.Enums;

public enum UserRole
{
    Admin,
    Employee,
    Client
}

public enum VehicleStatus
{
    Available,
    Rented,
    Maintenance
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Active,
    Completed,
    Cancelled
}
