using CredipathAPI.Data;
using CredipathAPI.Helpers;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace CredipathAPI.Services
{
    public class PermissionServices  
    {
        private readonly DataContext _context;

        public PermissionServices(DataContext context)
        {
            _context = context;
        }

        public async Task<PermissionDTO> CreatePermissionAsync(PermissionDTO permissionDTO)
        {
            var permission = new Permission
            {
                Module = permissionDTO.Module,
                Action = permissionDTO.Action
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return new PermissionDTO
            {
                Id = permission.Id,
                Module = permission.Module,
                Action = permission.Action
            };
        }

        public async Task<IEnumerable<PermissionDTO>> GetPermissionsAsync()
        {
            return await _context.Permissions
                .Select(p => new PermissionDTO
                {
                    Id = p.Id,
                    Module = p.Module,
                    Action = p.Action
                }).ToListAsync();
        }

        public async Task<PermissionDTO> GetPermissionByIdAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return null;

            return new PermissionDTO
            {
                Id = permission.Id,
                Module = permission.Module,
                Action = permission.Action
            };
        }


        public async Task<bool> UpdatePermissionAsync(int id, PermissionDTO permissionDTO)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return false;

            permission.Module = permissionDTO.Module;
            permission.Action = permissionDTO.Action;

            _context.Entry(permission).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePermissionAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return false;

            permission.Active = false;
            //_context.Permissions.Remove(permission);
            _context.Entry(permission).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
