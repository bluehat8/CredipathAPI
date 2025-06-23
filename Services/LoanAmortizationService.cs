using CredipathAPI.Data;
using CredipathAPI.DTOs;
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
                                                  PaymentNumber = la.PaymentNumber,
                                                  ClientId = u.Id,
                                                  ClientName = u.Name,
                                                  Code = u.Code
                                              }).ToListAsync();

            return overdueAmortizations;
        }


        public async Task<List<PaymentControlDTO>> GetPaymentsAsync(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                endDate = DateTime.Now;
                startDate = endDate.Value.AddMonths(12);
            }

            var paymentsQuery = from la in _context.LoanAmortization
                                join l in _context.Loans on la.LoanId equals l.Id
                                join u in _context.Clients on l.client_id equals u.Id
                                join r in _context.Routes on u.RouteId equals r.Id into routeGroup
                                from route in routeGroup.DefaultIfEmpty()
                                select new PaymentControlDTO
                                {
                                    LoanId = l.Id,
                                    PaymentAmount = la.PaymentAmount,
                                    PaymentDate = la.PaymentDate,
                                    PaymentStatus = Helper.GetPaymentStatusText(la.PaymentStatus),
                                    PayerName = u.Name,
                                    PaymentNumber = la.PaymentNumber,
                                    RouteName = route != null ? route.route_name : "Sin ruta asignada"
                                };

            paymentsQuery = paymentsQuery.Where(la => la.PaymentDate >= startDate && la.PaymentDate <= endDate);

            return await paymentsQuery.ToListAsync();
        }


        public async Task<List<LoanControlDTO>> GetLoansAsync(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                endDate = DateTime.Now;
                startDate = endDate.Value.AddMonths(-3);
            }

            var loansQuery = from l in _context.Loans
                             join u in _context.Clients on l.client_id equals u.Id
                             join r in _context.Routes on u.RouteId equals r.Id into routeGroup
                             from route in routeGroup.DefaultIfEmpty()
                             select new LoanControlDTO
                             {
                                 LoanId = l.Id,
                                 LoanAmount = l.amount,
                                 LoanDate = l.loan_date,
                                 LoanInstallments = l.installments,
                                 ClientName = u.Name,
                                 ClientCode = u.Code,
                                 RouteName = route != null ? route.route_name : "Sin ruta asignada"
                             };

            loansQuery = loansQuery.Where(l => l.LoanDate >= startDate && l.LoanDate <= endDate);

            return await loansQuery.ToListAsync();
        }



        public async Task<List<ExpenseControlDTO>> GetExpenseControlAsync(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                endDate = DateTime.Now;
                startDate = endDate.Value.AddMonths(-3);
            }

            // Listado de pagos (amortizaciones)
            var paymentsQuery = from la in _context.LoanAmortization
                                join l in _context.Loans on la.LoanId equals l.Id
                                join u in _context.Clients on l.client_id equals u.Id
                                join r in _context.Routes on u.RouteId equals r.Id into routeGroup
                                from route in routeGroup.DefaultIfEmpty()
                                select new ExpenseControlDTO
                                {
                                    LoanId = l.Id,
                                
                                    LoanAmount = l.amount,
                                    ClientName = u.Name,
                                    PaymentAmount = la.PaymentAmount,
                                    PaymentDate = la.PaymentDate,
                                    PaymentStatus = Helper.GetPaymentStatusText(la.PaymentStatus),
                                    PayerName = u.Name,
                                    TransactionType = "Pago",
                                    PaymentNumber = la.PaymentNumber,
                                    RouteName = route != null ? route.route_name : "Sin ruta asignada"
                                };

            paymentsQuery = paymentsQuery.Where(la => la.PaymentDate >= startDate && la.PaymentDate <= endDate);

            var payments = await paymentsQuery.ToListAsync();

            // Listado de préstamos realizados
            var loansQuery = from l in _context.Loans
                             join u in _context.Clients on l.client_id equals u.Id
                             join r in _context.Routes on u.RouteId equals r.Id into routeGroup
                             from route in routeGroup.DefaultIfEmpty()
                             select new ExpenseControlDTO
                             {
                                 LoanId = l.Id,
                                 LoanAmount = l.amount,
                                 ClientName = u.Name,
                                 PaymentAmount = 0, // No hay monto de pago para los préstamos
                                 PaymentDate = l.loan_date,
                                 PaymentStatus = Helper.GetPaymentStatusText(PaymentStatus.pending),
                                 PayerName = null, // No hay pagador en la creación del préstamo
                                 TransactionType = "Préstamo",
                                 RouteName = route != null ? route.route_name : "Sin ruta asignada"
                             };

            loansQuery = loansQuery.Where(l => l.PaymentDate >= startDate && l.PaymentDate <= endDate);

            var loans = await loansQuery.ToListAsync();

            var expenseControl = payments.Concat(loans).ToList();
            return expenseControl;
        }





    }
}
