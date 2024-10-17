using static CredipathAPI.Constants;

namespace CredipathAPI.Helpers
{
    public class ExpenseControlDTO
    {
        public int LoanId { get; set; }
        public decimal LoanAmount { get; set; }
        public string ClientName { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public string PayerName { get; set; }
        public string TransactionType { get; set; } 
    }
}
