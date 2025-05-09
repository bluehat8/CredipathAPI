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
            // Llave foránea FK_Clients_Routes_RouteId ya creada en una migración anterior. No es necesario agregarla de nuevo.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No se elimina la llave foránea aquí porque no fue agregada en esta migración.
        }
      }
}
