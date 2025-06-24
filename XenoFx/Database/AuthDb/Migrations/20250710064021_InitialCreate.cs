using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XenoFx.Database.AuthDb.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Sessions",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ClientId = table.Column<string>(type: "TEXT", nullable: false),
                AuthToken = table.Column<string>(type: "TEXT", nullable: true),
                AuthenticatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                ExpiresAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                RevokedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                Pin = table.Column<int>(type: "INTEGER", nullable: true),
                PinGenerationDisabledUntil = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                RetryDisabledUntil = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                IpAddress = table.Column<string>(type: "TEXT", nullable: true),
                ActivatedIpAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                DeactivatesIpAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                Notes = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sessions", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Sessions_ClientId",
            table: "Sessions",
            column: "ClientId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Sessions");
    }
}