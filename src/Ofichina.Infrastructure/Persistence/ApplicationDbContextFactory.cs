using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ofichina.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ofichinna;User Id=sa;Password=P@ssw0rd2024!Ofichina;TrustServerCertificate=True");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
// DefaultConnection": ;