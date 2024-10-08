using CredipathAPI.Data;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;
using static CredipathAPI.Constants;

namespace CredipathAPI.Services
{
    public class LoansService
    {
        private readonly DataContext _context;

        public LoansService(DataContext context)
        {
            _context = context;
        }

   
        public async Task<List<Loans>> GetAllLoans()
        {
            return await _context.Loans.ToListAsync();
        }

        public async Task<Loans>Getloanbyid(int id)
        {
            return await _context.Loans.FirstOrDefaultAsync(it => it.Id == id);
        }

  
        public async Task CreateLoanAsync(Loans loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync(); 

            await GenerateLoanAmortization(loan); 

            await _context.SaveChangesAsync();
        }

        private async Task GenerateLoanAmortization(Loans loan)
        {
            var amortizations = new List<LoanAmortization>();

            decimal loanAmount = loan.amount;
            decimal interestRate = loan.interst_rate / 100; 
            int totalInstallments = loan.installments;
            DateTime paymentDate = loan.loan_date;

            switch (loan.interest_type_id)
            {
                case 1: // Interés simple sobre el capital inicial
                    amortizations = GenerateSimpleInterestAmortization(loan.Id,loanAmount, interestRate, totalInstallments, paymentDate);
                    break;
                case 2: // Interés sobre cada cuota
                    amortizations = GenerateAmortizationWithInterestPerInstallment(loan.Id,loanAmount, interestRate, totalInstallments, paymentDate);
                    break;
                case 3: // Interés compuesto bancario
                    amortizations = GenerateCompoundInterestAmortization(loan.Id,loanAmount, interestRate, totalInstallments, paymentDate);
                    break;
                default:
                    throw new Exception("Tipo de interés no reconocido.");
            }

            _context.LoanAmortization.AddRange(amortizations);
        }

        private List<LoanAmortization> GenerateSimpleInterestAmortization(int loanId, decimal loanAmount, decimal interestRate, int installments, DateTime startDate)
        {
            var amortizations = new List<LoanAmortization>();
            decimal fixedPayment = loanAmount / installments;
            decimal totalInterest = loanAmount * interestRate;

            for (int i = 1; i <= installments; i++)
            {
                amortizations.Add(new LoanAmortization
                {
                    LoanId = loanId,
                    PaymentNumber = i,
                    PaymentDate = startDate.AddMonths(i), // Dependiendo de la frecuencia de pago
                    RealPaymentAmount = fixedPayment + totalInterest / installments,
                    InterestAmount = totalInterest / installments,
                    PaymentAmount = fixedPayment + totalInterest / installments,
                    PrincipalAmount = fixedPayment,
                    BalanceRemaining = loanAmount - (fixedPayment * i),
                    PaymentStatus = PaymentStatus.pending 
                });
            }

            return amortizations;
        }

        private List<LoanAmortization> GenerateAmortizationWithInterestPerInstallment(int loanId, decimal loanAmount, decimal interestRate, int installments, DateTime startDate)
        {
            var amortizations = new List<LoanAmortization>();
            decimal fixedPayment = loanAmount / installments;

            for (int i = 1; i <= installments; i++)
            {
                decimal interest = (loanAmount - (fixedPayment * (i - 1))) * interestRate;

                amortizations.Add(new LoanAmortization
                {
                    LoanId = loanId,
                    PaymentNumber = i,
                    PaymentDate = startDate.AddMonths(i),
                    RealPaymentAmount = fixedPayment + interest,
                    InterestAmount = interest,
                    PaymentAmount = fixedPayment + interest,
                    PrincipalAmount = fixedPayment,
                    BalanceRemaining = loanAmount - (fixedPayment * i),
                    PaymentStatus = PaymentStatus.pending
                });
            }

            return amortizations;
        }

        private List<LoanAmortization> GenerateCompoundInterestAmortization(int loanId, decimal loanAmount, decimal interestRate, int installments, DateTime startDate)
        {
            var amortizations = new List<LoanAmortization>();
            decimal compoundFactor = (decimal)Math.Pow(1 + (double)interestRate, installments);
            decimal fixedPayment = loanAmount * (interestRate * compoundFactor) / (compoundFactor - 1);

            for (int i = 1; i <= installments; i++)
            {
                decimal interest = loanAmount * interestRate;
                decimal principal = fixedPayment - interest;

                amortizations.Add(new LoanAmortization
                {
                    LoanId = loanId,
                    PaymentNumber = i,
                    PaymentDate = startDate.AddMonths(i),
                    RealPaymentAmount = fixedPayment,
                    InterestAmount = interest,
                    PaymentAmount = fixedPayment,
                    PrincipalAmount = principal,
                    BalanceRemaining = loanAmount - principal,
                    PaymentStatus = PaymentStatus.pending
                });

                loanAmount -= principal;
            }

            return amortizations;
        }

        public async Task<bool> UpdateLoanAsync(int id, Loans loan)
        {
            var existingloan = await _context.Loans.FindAsync(id);
            if (existingloan == null)
            {
                return false;
            }
            // Actualizar solo los campos que han cambiado
            if (existingloan.client_id != loan.client_id)
            {
                existingloan.client_id = loan.client_id;
            }

            if (existingloan.amount != loan.amount)
            {
                existingloan.amount = loan.amount;
            }

            if (existingloan.interest_type_id != loan.interest_type_id)
            {
                existingloan.interest_type_id = loan.interest_type_id;
            }

            if (existingloan.interst_rate != loan.interst_rate)
            {
                existingloan.interst_rate = loan.interst_rate;
            }

            if (existingloan.installments != loan.installments)
            {
                existingloan.installments = loan.installments;
            }

            if (existingloan.frecuency_id != loan.frecuency_id)
            {
                existingloan.frecuency_id = loan.frecuency_id;
            }

            if (existingloan.custom_payment_interval != loan.custom_payment_interval)
            {
                existingloan.custom_payment_interval = loan.custom_payment_interval;
            }

            if (existingloan.loan_date != loan.loan_date)
            {
                existingloan.loan_date = loan.loan_date;
            }

            if (existingloan.notes != loan.notes)
            {
                existingloan.notes = loan.notes;
            }

            existingloan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteLoanAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return false;
            }

            loan.Active = false;
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
