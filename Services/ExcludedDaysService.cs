using CredipathAPI.Data;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace CredipathAPI.Services
{
    public class ExcludedDaysService
    {

        private readonly DataContext _context;

        public ExcludedDaysService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ExcludedDays>> GetExcludedDays()
        {
            return await _context.excludedDays.ToListAsync();
        }

        public async Task<ExcludedDays> GetExcludedDaysbyid(int id)
        {
            return await _context.excludedDays.FirstOrDefaultAsync(it => it.Id == id);
        }

        public async Task CreateEcxcludedDayAsync(ExcludedDays obj)
        {
            _context.excludedDays.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateExcludedDayAsync(int id, ExcludedDays excludedDays)
        {
            var existingday = await _context.excludedDays.FindAsync(id);
            if (existingday == null)
            {
                return false;
            }
            // Actualizar solo los campos que han cambiado
            if (existingday.excludes_day_name != excludedDays.excludes_day_name)
            {
                existingday.excludes_day_name = existingday.excludes_day_name;
            }

            if (existingday.loan_id != excludedDays.loan_id)
            {
                existingday.loan_id = existingday.loan_id;
            }

            existingday.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteExcludedDayAsync(int id)
        {
            var day = await _context.excludedDays.FindAsync(id);
            if (day == null)
            {
                return false;
            }

            day.Active = false;
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
