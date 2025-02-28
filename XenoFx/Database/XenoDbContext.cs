using Microsoft.EntityFrameworkCore;
using XenoFx.Database.Models;

namespace XenoFx.Database;

// Rename this class to XenoAssetsContext

/// <summary>
/// Database storage model for integrity and recovery metadata of a file repository.
/// </summary>
/// <param name="options">Database Connection and Runtime Options</param>
public partial class XenoDbContext(DbContextOptions<XenoDbContext> options) : DbContext(options)
{
    // Tables

    public DbSet<AssetRecord> MetadataRecords { get; set; }
}