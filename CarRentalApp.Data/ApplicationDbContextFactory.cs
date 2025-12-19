using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CarRentalApp.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        // Read from env or fallback to LocalDB
        var conn = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")
                   ?? "Server=(localdb)\\MSSQLLocalDB;Database=CarRentalDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        builder.UseSqlServer(conn);
        return new ApplicationDbContext(builder.Options);
    }
}
