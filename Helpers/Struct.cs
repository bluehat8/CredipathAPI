namespace CredipathAPI.Helpers
{
    public class Struct
    {
        public class AmortizationParams
        {
            public int LoanId { get; set; }
            public decimal LoanAmount { get; set; }
            public decimal InterestRate { get; set; }
            public int Installments { get; set; }
            public DateTime StartDate { get; set; }
            public string FrequencyName { get; set; }
        }
    }
}
