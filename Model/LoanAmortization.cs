using CredipathAPI.Base;
using static CredipathAPI.Constants;

namespace CredipathAPI.Model
{
    public class LoanAmortization:BaseEntity
    {
        public int LoanId { get; set; }
        public int PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal RealPaymentAmount { get; set; } // Fixed payment amount
        public decimal InterestAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal BalanceRemaining { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.pending;
        //public Loans Loan { get; set; }
    }

}
