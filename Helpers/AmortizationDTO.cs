using static CredipathAPI.Constants;

namespace CredipathAPI.Helpers
{
    public class AmortizationDTO
    {
        public int AmortizationId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int LoanId { get; set; }
        public decimal LoanAmount { get; set; }
        public int ClientId { get; set; }
        public string Code { get; set; }
        public string ClientName { get; set; }
    }
}
