using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CredipathAPI.Migrations
{
    /// <inheritdoc />
    public partial class ADDEXCLUDEDDAYS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Clients_ClientId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_InterestTypes_InterestTypesId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_paymentfrequencies_PaymentfrequenciesId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_ClientId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_InterestTypesId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_PaymentfrequenciesId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "InterestTypesId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PaymentfrequenciesId",
                table: "Loans");

            migrationBuilder.CreateTable(
                name: "excludedDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    excludes_day_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    loan_id = table.Column<int>(type: "int", nullable: false),
                    loansId = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_excludedDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_excludedDays_Loans_loansId",
                        column: x => x.loansId,
                        principalTable: "Loans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_excludedDays_loansId",
                table: "excludedDays",
                column: "loansId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "excludedDays");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Loans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterestTypesId",
                table: "Loans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentfrequenciesId",
                table: "Loans",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Clients_ClientId",
                table: "Loans",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_InterestTypes_InterestTypesId",
                table: "Loans",
                column: "InterestTypesId",
                principalTable: "InterestTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_paymentfrequencies_PaymentfrequenciesId",
                table: "Loans",
                column: "PaymentfrequenciesId",
                principalTable: "paymentfrequencies",
                principalColumn: "Id");
        }
    }
}
