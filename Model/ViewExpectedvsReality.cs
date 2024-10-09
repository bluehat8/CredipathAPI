using Microsoft.EntityFrameworkCore;

namespace CredipathAPI.Model
{

    [Keyless]
    public class ViewExpectedvsReality
    {
        public int LoanId { get; set; }
        public int PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal RealPaymentAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal BalanceRemaining { get; set; }
        public string State { get; set; }
        public string ClientName { get; set; }  


    }
}
