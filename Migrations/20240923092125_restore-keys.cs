using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CredipathAPI.Migrations
{
    /// <inheritdoc />
    public partial class restorekeys : Migration
    {
        /// <inheritdoc />
            /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar la llave foránea RouteId en Clients
            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Routes_RouteId",
                table: "Clients",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull // Permite que Route sea opcional
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar la llave foránea si se hace rollback
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Routes_RouteId",
                table: "Clients"
            );
        }
      }
}
