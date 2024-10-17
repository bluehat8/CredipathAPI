using CredipathAPI.Data;
using CredipathAPI.Helpers;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;
using static CredipathAPI.Constants;

namespace CredipathAPI.Services
{
    public class LoanAmortizationService
    {
        private readonly DataContext _context;

        public LoanAmortizationService(DataContext context)
        {
            _context = context;
        }


        public async Task<bool> SetOverdueAmortizationsAsync()
        {
            //var overdueAmortizations = await _context.LoanAmortization
            //    .Where(la => la.PaymentStatus != PaymentStatus.paid && la.PaymentDate < DateTime.Now)
            //    .ToListAsync();

            var overdueAmortizations = await _context.LoanAmortization
                .Where(la => la.PaymentStatus != PaymentStatus.paid
                             && (la.PaymentDate < DateTime.Now || la.PaymentStatus == PaymentStatus.overdue))
                .ToListAsync();

            bool hasChanges = false;

            foreach (var amortization in overdueAmortizations)
            {
                if (amortization.PaymentStatus != PaymentStatus.overdue)
                {
                    amortization.PaymentStatus = PaymentStatus.overdue;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await _context.SaveChangesAsync();
            }

            return hasChanges;
        }


        public async Task<List<AmortizationDTO>> GetOverdueAmortizationsAsync()
        {
            var overdueAmortizations = await (from la in _context.LoanAmortization
                                              where (la.PaymentStatus != PaymentStatus.paid
                                                     && la.PaymentDate < DateTime.Now)
                                                     || la.PaymentStatus == PaymentStatus.overdue
                                              join l in _context.Loans on la.LoanId equals l.Id
                                              join u in _context.Clients on l.client_id equals u.Id
                                              select new AmortizationDTO
                                              {
                                                  AmortizationId = la.Id,
                                                  PaymentAmount = la.PaymentAmount,
                                                  PaymentDate = la.PaymentDate,
                                                  PaymentStatus = la.PaymentStatus,
                                                  LoanId = la.LoanId,
                                                  LoanAmount = l.amount,
                                                  ClientId = u.Id,
                                                  ClientName = u.name,
                                                  Code = u.code
                                              }).ToListAsync();

            return overdueAmortizations;
        }


        public async Task<List<ExpenseControlDTO>> GetExpenseControlAsync(DateTime? startDate, DateTime? endDate)
        {
            // Listado de pagos (amortizaciones)
            var paymentsQuery = from la in _context.LoanAmortization
                                join l in _context.Loans on la.LoanId equals l.Id
                                join u in _context.Clients on l.client_id equals u.Id
                                select new ExpenseControlDTO
                                {
                                    LoanId = l.Id,
                                    LoanAmount = l.amount,
                                    ClientName = u.name,
                                    PaymentAmount = la.PaymentAmount,
                                    PaymentDate = la.PaymentDate,
                                    PaymentStatus = Helper.GetPaymentStatusText(la.PaymentStatus),
                                    PayerName = u.name,
                                    TransactionType = "Pago"
                                };

            if (startDate.HasValue && endDate.HasValue)
            {
                paymentsQuery = paymentsQuery.Where(la => la.PaymentDate >= startDate && la.PaymentDate <= endDate);
            }

            var payments = await paymentsQuery.ToListAsync();

            // Listado de préstamos realizados
            var loansQuery = from l in _context.Loans
                             join u in _context.Clients on l.client_id equals u.Id
                             select new ExpenseControlDTO
                             {
                                 LoanId = l.Id,
                                 LoanAmount = l.amount,
                                 ClientName = u.name,
                                 PaymentAmount = 0, 
                                 PaymentDate = l.loan_date,
                                 PaymentStatus = Helper.GetPaymentStatusText(PaymentStatus.pending),
                                 PayerName = null,
                                 TransactionType = "Préstamo"
                             };

            if (startDate.HasValue && endDate.HasValue)
            {
                loansQuery = loansQuery.Where(l => l.PaymentDate >= startDate && l.PaymentDate <= endDate);
            }

            var loans = await loansQuery.ToListAsync();

            var expenseControl = payments.Concat(loans).ToList();
            //s
            return expenseControl;
        }




    }
}
