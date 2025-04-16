using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace XenoFx.Database.CacheDb;

// Allows creating migration checkpoints without incident.
public class CacheDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CacheDbContext>
{
    public CacheDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CacheDbContext>();
        optionsBuilder.UseSqlite("Data Source=Cache.db");
        return new CacheDbContext(optionsBuilder.Options);
    }
}