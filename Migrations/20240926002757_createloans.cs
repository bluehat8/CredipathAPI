using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CredipathAPI.Migrations
{
    /// <inheritdoc />
    public partial class createloans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    interest_type_id = table.Column<int>(type: "int", nullable: false),
                    interst_rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    installments = table.Column<int>(type: "int", nullable: false),
                    frecuency_id = table.Column<int>(type: "int", nullable: false),
                    custom_payment_interval = table.Column<int>(type: "int", nullable: false),
                    loan_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    InterestTypesId = table.Column<int>(type: "int", nullable: true),
                    PaymentfrequenciesId = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Loans_InterestTypes_InterestTypesId",
                        column: x => x.InterestTypesId,
                        principalTable: "InterestTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Loans_paymentfrequencies_PaymentfrequenciesId",
                        column: x => x.PaymentfrequenciesId,
                        principalTable: "paymentfrequencies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ClientId",
                table: "Loans",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_InterestTypesId",
                table: "Loans",
                column: "InterestTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_PaymentfrequenciesId",
                table: "Loans",
                column: "PaymentfrequenciesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loans");
        }
    }
}
