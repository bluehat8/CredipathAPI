using CredipathAPI.Data;
using CredipathAPI.Helpers;
using CredipathAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace CredipathAPI.Services
{
    public class BankService
    {
        private readonly DataContext _context;

        public BankService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Bank>> GetAllBankAsync()
        {
            return await _context.Banks.ToListAsync();
        }

        public async Task<Bank> GetBankIdAsync(int id)
        {
            return await _context.Banks.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateBankAsync(Bank bank)
        {
            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateBankAsync(Bank bank)
        {

            _context.Banks.Update(bank);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBankAsync(int id)
        {
            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return false;
            }

            bank.Active = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public bool BankExists(int id)
        {
            return _context.Banks.Any(e => e.Id == id);
        }




    }
}
