using Microsoft.EntityFrameworkCore;
using XenoFx.Database.Assets.Models;

namespace XenoFx.Database.Assets;

/// <summary>
/// Database storage model for integrity and recovery metadata of a file repository.
/// </summary>
/// <param name="options">Database Connection and Runtime Options</param>
public partial class AssetsDbContext(DbContextOptions<AssetsDbContext> options) : DbContext(options)
{
    // Tables

    public DbSet<AssetRecord> MetadataRecords { get; set; }
}