using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CSX.DotNet.Modules.AuthIpPin.Storage.AuthDb;

// Allows creating migration checkpoints without incident.
public class AuthDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseSqlite("Data Source=Auth.db");
        return new AuthDbContext(optionsBuilder.Options);
    }
}