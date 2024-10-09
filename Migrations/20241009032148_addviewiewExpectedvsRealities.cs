using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CredipathAPI.Migrations
{
    /// <inheritdoc />
    public partial class addviewiewExpectedvsRealities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
            CREATE VIEW ViewExpectedvsReality AS
            SELECT 
               LA.LoanId,
               LA.PaymentNumber,
               LA.PaymentDate,
               LA.RealPaymentAmount,
               LA.InterestAmount,
               LA.PaymentAmount,
               LA.PrincipalAmount,
               LA.BalanceRemaining,
               'paid' AS State,
               C.name as ClientName
            FROM dbo.LoanAmortization LA
            INNER JOIN Loans L 
               ON L.Id = LA.LoanId
            INNER JOIN Clients C
               ON L.client_id = C.Id
            WHERE LA.PaymentStatus = 1 -- Pagados

            UNION ALL

            SELECT 
               LA.LoanId,
               LA.PaymentNumber,
               LA.PaymentDate,
               LA.RealPaymentAmount,
               LA.InterestAmount,
               LA.PaymentAmount,
               LA.PrincipalAmount,
               LA.BalanceRemaining,
               'unpaid' AS State,
               C.name as ClientName
            FROM dbo.LoanAmortization LA
            INNER JOIN Loans L 
               ON L.Id = LA.LoanId
            INNER JOIN Clients C
               ON L.client_id = C.Id
            WHERE LA.PaymentStatus = 0 -- No pagados
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "viewExpectedvsRealities");
        }
    }
}
