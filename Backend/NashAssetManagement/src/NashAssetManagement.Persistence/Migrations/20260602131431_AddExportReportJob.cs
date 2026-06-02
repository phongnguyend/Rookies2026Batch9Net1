using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NashAssetManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExportReportJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExportReportJobs",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportReportJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportReportJobs_Users_RequestedByAdminId",
                        column: x => x.RequestedByAdminId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExportReportJobs_RequestedByAdminId",
                schema: "Core",
                table: "ExportReportJobs",
                column: "RequestedByAdminId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportReportJobs",
                schema: "Core");
        }
    }
}
