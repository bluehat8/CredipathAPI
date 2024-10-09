using CredipathAPI.Data;
using CredipathAPI.Helpers;
using CredipathAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CredipathAPI.Services
{
    public class ViewExpectedvsRealityService
    {

        private readonly DataContext _context;

        public ViewExpectedvsRealityService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Model.ViewExpectedvsReality>> GetViewExpectedvsRealityAsync()
        {
            return await _context.viewExpectedvsRealities.FromSqlRaw(@"
            SELECT 
                LoanId,
                PaymentNumber,
                PaymentDate,
                RealPaymentAmount,
                InterestAmount,
                PaymentAmount,
                PrincipalAmount,
                BalanceRemaining,
                State,
                ClientName
            FROM ViewExpectedvsReality
        ").ToListAsync();
        }

    }
}
