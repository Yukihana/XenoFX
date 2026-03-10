using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging.Abstractions;

namespace XenoFx.Database.AssetsDb;

// Allows creating migration checkpoints without incident
public class AssetsDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AssetsDbContext>
{
    public AssetsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AssetsDbContext>();
        optionsBuilder.UseSqlite("Data Source=Assets.db");

        return new AssetsDbContext(
            optionsBuilder.Options,
            NullLogger<AssetsDbContext>.Instance);
    }
}