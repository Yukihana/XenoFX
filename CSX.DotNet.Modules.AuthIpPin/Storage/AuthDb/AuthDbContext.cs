using CSX.DotNet.Modules.AuthIpPin.Storage.AuthDb.Models;
using Microsoft.EntityFrameworkCore;

namespace CSX.DotNet.Modules.AuthIpPin.Storage.AuthDb;

public class AuthDbContext : DbContext
{
    public DbSet<IpPinAuthSession> Sessions { get; set; } = null!;

    // Lifetime

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    { }

    // Encoding/Decoding setup

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Conversions

        // Register unique columns

        modelBuilder.Entity<IpPinAuthSession>()
            .HasIndex(x => x.ClientId)
            .IsUnique();
    }
}