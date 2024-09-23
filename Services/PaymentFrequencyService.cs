using CredipathAPI.Data;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace CredipathAPI.Services
{
    public class PaymentFrequencyService
    {

        private readonly DataContext _context;

        public PaymentFrequencyService(DataContext context)
        {
            _context = context;
        }

        // Obtener todos los tipos de interés
        public async Task<List<Paymentfrequencies>> GetAllPaymenteFrequency()
        {
            return await _context.paymentfrequencies.ToListAsync();
        }

        // Obtener un tipo de interés por su ID
        public async Task<Paymentfrequencies> GetPaymentFrequencybyid(int id)
        {
            return await _context.paymentfrequencies.FirstOrDefaultAsync(it => it.Id == id);
        }

        // Crear un nuevo tipo de interés
        public async Task CreatePaymentFrequencyAsync(Paymentfrequencies paymentfrequencies)
        {
            _context.paymentfrequencies.Add(paymentfrequencies);
            await _context.SaveChangesAsync();
        }


        // Actualizar campos específicos del tipo de interés
        public async Task<bool> UpdatePaymentFrequencyFieldsAsync(int id, Paymentfrequencies paymentfrequencies)
        {
            var existingPay = await _context.paymentfrequencies.FindAsync(id);
            if (existingPay == null)
            {
                return false;
            }

            // Actualizar solo los campos que han cambiado
            if (existingPay.frequency_name != "String")
            {
                existingPay.frequency_name = paymentfrequencies.frequency_name;
            }

            existingPay.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar (suave) un tipo de interés (marcarlo como inactivo)
        public async Task<bool> SoftDeleteInterestTypeAsync(int id)
        {
            var pay = await _context.paymentfrequencies.FindAsync(id);
            if (pay == null)
            {
                return false;
            }

            pay.Active = false; // Suponiendo que hay un campo `Active` en `BaseEntity`
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
