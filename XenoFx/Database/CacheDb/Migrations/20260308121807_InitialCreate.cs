using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XenoFx.Database.CacheDb.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AssetPresences",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                NormalizedPath = table.Column<string>(type: "TEXT", nullable: false),
                OriginalPath = table.Column<string>(type: "TEXT", nullable: false),
                AssetId = table.Column<byte[]>(type: "BLOB", nullable: false),
                StateIndex = table.Column<ulong>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssetPresences", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AssetPresences_NormalizedPath",
            table: "AssetPresences",
            column: "NormalizedPath",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AssetPresences");
    }
}