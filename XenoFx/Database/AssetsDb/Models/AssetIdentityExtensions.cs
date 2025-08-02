using CSX.DotNet.EFC.Common.Columns;
using Microsoft.EntityFrameworkCore;

namespace XenoFx.Database.AssetsDb.Models;

public static class AssetIdentityExtensions
{
    public static ModelBuilder SpecifyAssetIdentity(
        this ModelBuilder modelBuilder)
    {
        // Get database type for column compatibity
        var dbProvider = modelBuilder
            .Model.FindAnnotation("Relational:ProviderName")?
            .Value as string
            ?? string.Empty;

        // Mark primary key
        modelBuilder.Entity<AssetIdentity>()
            .HasKey(x => x.Id);

        // Attach primary key conversion
        modelBuilder.Entity<AssetIdentity>()
            .Property(x => x.Id)
            .HasConversion(GuidByteBuilder.Converter)
            .HasColumnType(GuidByteBuilder.GetColumnType(dbProvider));

        return modelBuilder;
    }
}