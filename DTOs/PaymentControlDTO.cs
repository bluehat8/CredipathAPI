namespace CredipathAPI.DTOs
{
    public class PaymentControlDTO
    {
        public int LoanId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PayerName { get; set; }
        public int PaymentNumber { get; set; }
        public string? RouteName { get; set; }
    }
}
