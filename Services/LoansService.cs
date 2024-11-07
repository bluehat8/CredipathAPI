using CredipathAPI.Data;
using CredipathAPI.Helpers;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;
using static CredipathAPI.Constants;
using static CredipathAPI.Helpers.Struct;

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
            var frequency = await _context.paymentfrequencies.FindAsync(loan.frecuency_id);
            if (frequency == null)
            {
                throw new Exception("Frecuencia de pago no encontrada.");
            }

            var amortizationParams = new AmortizationParams
            {
                LoanId = loan.Id,
                LoanAmount = loan.amount,
                InterestRate = loan.interst_rate / 100,
                Installments = loan.installments,
                StartDate = loan.loan_date,
                FrequencyName = frequency.frequency_name
            };

            List<LoanAmortization> amortizations = loan.interest_type_id switch
            {
                1 => GenerateSimpleInterestAmortization(amortizationParams),
                2 => GenerateAmortizationWithInterestPerInstallment(amortizationParams),
                3 => GenerateCompoundInterestAmortization(amortizationParams),
                _ => throw new Exception("Tipo de interés no reconocido.")
            };

            _context.LoanAmortization.AddRange(amortizations);
        }


        private List<LoanAmortization> GenerateSimpleInterestAmortization(AmortizationParams parameters)
        {
            var amortizations = new List<LoanAmortization>();
            decimal fixedPayment = parameters.LoanAmount / parameters.Installments;
            decimal totalInterest = parameters.LoanAmount * parameters.InterestRate;

            for (int i = 1; i <= parameters.Installments; i++)
            {
                amortizations.Add(new LoanAmortization
                {
                    LoanId = parameters.LoanId,
                    PaymentNumber = i,
                    PaymentDate = Helper.CalculateNextPaymentDate(parameters.StartDate, i, parameters.FrequencyName),
                    RealPaymentAmount = fixedPayment + totalInterest / parameters.Installments,
                    InterestAmount = totalInterest / parameters.Installments,
                    PaymentAmount = fixedPayment + totalInterest / parameters.Installments,
                    PrincipalAmount = fixedPayment,
                    BalanceRemaining = parameters.LoanAmount - (fixedPayment * i),
                    PaymentStatus = PaymentStatus.pending
                });
            }

            return amortizations;
        }

        private List<LoanAmortization> GenerateAmortizationWithInterestPerInstallment(AmortizationParams parameters)
        {
            var amortizations = new List<LoanAmortization>();
            decimal fixedPayment = parameters.LoanAmount / parameters.Installments;

            for (int i = 1; i <= parameters.Installments; i++)
            {
                decimal interest = (parameters.LoanAmount - (fixedPayment * (i - 1))) * parameters.InterestRate;

                amortizations.Add(new LoanAmortization
                {
                    LoanId = parameters.LoanId,
                    PaymentNumber = i,
                    PaymentDate = Helper.CalculateNextPaymentDate(parameters.StartDate, i, parameters.FrequencyName),
                    RealPaymentAmount = fixedPayment + interest,
                    InterestAmount = interest,
                    PaymentAmount = fixedPayment + interest,
                    PrincipalAmount = fixedPayment,
                    BalanceRemaining = parameters.LoanAmount - (fixedPayment * i),
                    PaymentStatus = PaymentStatus.pending
                });
            }

            return amortizations;
        }

        private List<LoanAmortization> GenerateCompoundInterestAmortization(AmortizationParams parameters)
        {
            var amortizations = new List<LoanAmortization>();
            decimal compoundFactor = (decimal)Math.Pow(1 + (double)parameters.InterestRate, parameters.Installments);
            decimal fixedPayment = parameters.LoanAmount * (parameters.InterestRate * compoundFactor) / (compoundFactor - 1);

            for (int i = 1; i <= parameters.Installments; i++)
            {
                decimal interest = parameters.LoanAmount * parameters.InterestRate;
                decimal principal = fixedPayment - interest;

                amortizations.Add(new LoanAmortization
                {
                    LoanId = parameters.LoanId,
                    PaymentNumber = i,
                    PaymentDate = Helper.CalculateNextPaymentDate(parameters.StartDate, i, parameters.FrequencyName),
                    RealPaymentAmount = fixedPayment,
                    InterestAmount = interest,
                    PaymentAmount = fixedPayment,
                    PrincipalAmount = principal,
                    BalanceRemaining = parameters.LoanAmount - principal,
                    PaymentStatus = PaymentStatus.pending
                });

                parameters.LoanAmount -= principal;
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

            var existingAmortizations = await _context.LoanAmortization.Where(a => a.LoanId == id).ToListAsync();
            _context.LoanAmortization.RemoveRange(existingAmortizations);
            await _context.SaveChangesAsync();

            //Regenerando las amortizaciones
            await GenerateLoanAmortization(existingloan);

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

            // Desactivando las amortizaciones asociadas
            var amortizations = await _context.LoanAmortization
                .Where(a => a.LoanId == id)
                .ToListAsync();

            foreach (var amortization in amortizations)
            {
                amortization.Active = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }



    }
}
