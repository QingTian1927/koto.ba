using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kotoba.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Server=WORKBOX;Database=KotobaDb;User Id=sa;Password=123;TrustServerCertificate=true;MultipleActiveResultSets=true");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
