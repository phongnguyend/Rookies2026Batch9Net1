using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NashAssetManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokensConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                schema: "auth",
                table: "refresh_tokens",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                schema: "auth",
                table: "refresh_tokens");
        }
    }
}
