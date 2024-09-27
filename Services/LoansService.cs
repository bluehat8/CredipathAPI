using CredipathAPI.Data;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;

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

        public async Task CreateloanAsync(Loans loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
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
