namespace CredipathAPI.DTOs
{
    public class LoanControlDTO
    {
        public int LoanId { get; set; }
        public decimal LoanAmount { get; set; }
        public DateTime LoanDate { get; set; }
        public string? ClientName { get; set; }
        public string? RouteName { get; set; }
        public string? ClientCode { get; set; }
        public int LoanInstallments { get; set; }
    }
}
