using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CredipathAPI.Migrations
{
    /// <inheritdoc />
    public partial class Restore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Permissions");
        }
    }
}
