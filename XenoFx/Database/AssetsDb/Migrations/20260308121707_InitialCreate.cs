using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace XenoFx.Database.AssetsDb.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Description",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                Title = table.Column<string>(type: "TEXT", nullable: false),
                Note = table.Column<string>(type: "TEXT", nullable: false),
                Tags = table.Column<string>(type: "TEXT", nullable: false),
                OriginalTitle = table.Column<string>(type: "TEXT", nullable: false),
                PageUrls = table.Column<string>(type: "TEXT", nullable: false),
                DataUrls = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Description", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Identities",
            columns: table => new
            {
                Id = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                FileCreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                FileModifiedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                SHA256 = table.Column<byte[]>(type: "BLOB", nullable: false),
                Blake3 = table.Column<byte[]>(type: "BLOB", nullable: false),
                Crumbs = table.Column<byte[]>(type: "BLOB", nullable: false),
                RecordCreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                RecordCreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                RecordUpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                RecordUpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                IsRecordDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                RecordDeletedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                RecordDeletedBy = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Identities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Media",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Media", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Presentation",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Presentation", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Tags",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                TagId = table.Column<string>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tags", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Tags_TagId",
            table: "Tags",
            column: "TagId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Description");

        migrationBuilder.DropTable(
            name: "Identities");

        migrationBuilder.DropTable(
            name: "Media");

        migrationBuilder.DropTable(
            name: "Presentation");

        migrationBuilder.DropTable(
            name: "Tags");
    }
}