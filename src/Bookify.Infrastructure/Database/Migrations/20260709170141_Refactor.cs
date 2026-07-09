using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "roles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "role_user",
                newName: "role_user",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "role_permissions",
                newName: "role_permissions",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "reviews",
                newName: "reviews",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "permissions",
                newName: "permissions",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "outbox_messages",
                newName: "outbox_messages",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "bookings",
                newName: "bookings",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "apartments",
                newName: "apartments",
                newSchema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "users",
                schema: "public",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "roles",
                schema: "public",
                newName: "roles");

            migrationBuilder.RenameTable(
                name: "role_user",
                schema: "public",
                newName: "role_user");

            migrationBuilder.RenameTable(
                name: "role_permissions",
                schema: "public",
                newName: "role_permissions");

            migrationBuilder.RenameTable(
                name: "reviews",
                schema: "public",
                newName: "reviews");

            migrationBuilder.RenameTable(
                name: "permissions",
                schema: "public",
                newName: "permissions");

            migrationBuilder.RenameTable(
                name: "outbox_messages",
                schema: "public",
                newName: "outbox_messages");

            migrationBuilder.RenameTable(
                name: "bookings",
                schema: "public",
                newName: "bookings");

            migrationBuilder.RenameTable(
                name: "apartments",
                schema: "public",
                newName: "apartments");
        }
    }
}
