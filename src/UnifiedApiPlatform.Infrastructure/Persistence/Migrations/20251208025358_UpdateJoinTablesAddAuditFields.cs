using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace UnifiedApiPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJoinTablesAddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "user_roles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "user_roles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Instant>(
                name: "updated_at",
                table: "user_roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "user_roles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "role_permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "role_permissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Instant>(
                name: "updated_at",
                table: "role_permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "role_permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "role_menus",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "role_menus",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Instant>(
                name: "updated_at",
                table: "role_menus",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "role_menus",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_by",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "id",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "id",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "role_menus");

            migrationBuilder.DropColumn(
                name: "id",
                table: "role_menus");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "role_menus");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "role_menus");
        }
    }
}
