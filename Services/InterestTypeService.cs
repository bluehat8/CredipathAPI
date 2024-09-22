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
    public class InterestTypeService
    {

        private readonly DataContext _context;

        public InterestTypeService(DataContext context)
        {
            _context = context;
        }

        // Obtener todos los tipos de interés
        public async Task<List<InterestTypes>> GetAllInterestTypesAsync()
        {
            return await _context.InterestTypes.ToListAsync();
        }

        // Obtener un tipo de interés por su ID
        public async Task<InterestTypes> GetInterestTypeByIdAsync(int id)
        {
            return await _context.InterestTypes.FirstOrDefaultAsync(it => it.Id == id);
        }

        // Crear un nuevo tipo de interés
        public async Task CreateInterestTypeAsync(InterestTypes interestType)
        {
            _context.InterestTypes.Add(interestType);
            await _context.SaveChangesAsync();
        }

        // Actualizar un tipo de interés existente
        public async Task UpdateInterestTypeAsync(InterestTypes interestType)
        {
            _context.InterestTypes.Update(interestType);
            await _context.SaveChangesAsync();
        }

        // Actualizar campos específicos del tipo de interés
        public async Task<bool> UpdateInterestTypeFieldsAsync(int id, InterestTypes updatedInterestType)
        {
            var existingInterestType = await _context.InterestTypes.FindAsync(id);
            if (existingInterestType == null)
            {
                return false;
            }

            // Actualizar solo los campos que han cambiado
            if (updatedInterestType.Interest_Type_Name !="String")
            {
                existingInterestType.Interest_Type_Name = updatedInterestType.Interest_Type_Name;
            }

            existingInterestType.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar (suave) un tipo de interés (marcarlo como inactivo)
        public async Task<bool> SoftDeleteInterestTypeAsync(int id)
        {
            var interestType = await _context.InterestTypes.FindAsync(id);
            if (interestType == null)
            {
                return false;
            }

            interestType.Active = false; // Suponiendo que hay un campo `Active` en `BaseEntity`
            await _context.SaveChangesAsync();
            return true;
        }

        // Comprobar si un tipo de interés existe
        public bool InterestTypeExists(int id)
        {
            return _context.InterestTypes.Any(e => e.Id == id);
        }

    }
}
