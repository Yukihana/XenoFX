using Microsoft.EntityFrameworkCore;

namespace XenoFx.Database.AuthDb;

public class AuthDbContext : DbContext
{
    public DbSet<Models.IpPinAuthSession> Sessions { get; set; } = null!;

    // Lifetime

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    { }

    // Encoding/Decoding setup

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Conversions

        // Register unique columns

        modelBuilder.Entity<Models.IpPinAuthSession>()
            .HasIndex(x => x.ClientId)
            .IsUnique();
    }
}