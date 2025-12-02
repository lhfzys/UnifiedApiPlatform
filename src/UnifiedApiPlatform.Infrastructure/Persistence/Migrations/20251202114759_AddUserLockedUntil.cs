using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnifiedApiPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLockedUntil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "failed_login_attempts",
                table: "users",
                newName: "login_failure_count");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "login_failure_count",
                table: "users",
                newName: "failed_login_attempts");
        }
    }
}
