using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NashAssetManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssetAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at_utc",
                schema: "core",
                table: "assignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "core",
                table: "assignments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at_utc",
                schema: "core",
                table: "assets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at_utc",
                schema: "core",
                table: "assets",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deleted_at_utc",
                schema: "core",
                table: "assignments");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "core",
                table: "assignments");

            migrationBuilder.DropColumn(
                name: "created_at_utc",
                schema: "core",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "updated_at_utc",
                schema: "core",
                table: "assets");
        }
    }
}
