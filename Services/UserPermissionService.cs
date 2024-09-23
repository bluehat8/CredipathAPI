using CredipathAPI.Data;
using CredipathAPI.Helpers;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace CredipathAPI.Services
{
    public class UserPermissionService
    {
        private readonly DataContext _context;

        public UserPermissionService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserPermissionDTO>> GetUserPermissionsAsync()
        {
            return await _context.UserPermissions
                .Include(up => up.Permission)
                .Select(up => new UserPermissionDTO
                {
                    Id = up.Id,
                    UserId = up.UserId,
                    PermissionId = up.PermissionId,
                    PermissionModule = up.Permission.Module,
                    PermissionAction = up.Permission.Action
                }).ToListAsync();
        }

        public async Task<UserPermissionDTO> GetUserPermissionByIdAsync(int id)
        {
            var userPermission = await _context.UserPermissions
                .Include(up => up.Permission)
                .FirstOrDefaultAsync(up => up.Id == id);

            if (userPermission == null) return null;

            return new UserPermissionDTO
            {
                Id = userPermission.Id,
                UserId = userPermission.UserId,
                PermissionId = userPermission.PermissionId,
                PermissionModule = userPermission.Permission.Module,
                PermissionAction = userPermission.Permission.Action
            };
        }

        public async Task<UserPermissionDTO> CreateUserPermissionAsync(UserPermissionDTO userPermissionDTO)
        {
            var userPermission = new UserPermission
            {
                UserId = userPermissionDTO.UserId,
                PermissionId = userPermissionDTO.PermissionId
            };

            _context.UserPermissions.Add(userPermission);
            await _context.SaveChangesAsync();

            userPermissionDTO.Id = userPermission.Id;
            return userPermissionDTO;
        }

        public async Task<bool> UpdateUserPermissionAsync(int id, UserPermissionDTO userPermissionDTO)
        {
            var userPermission = await _context.UserPermissions.FindAsync(id);
            if (userPermission == null) return false;

            userPermission.UserId = userPermissionDTO.UserId;
            userPermission.PermissionId = userPermissionDTO.PermissionId;

            _context.Entry(userPermission).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserPermissionAsync(int id)
        {
            var userPermission = await _context.UserPermissions.FindAsync(id);
            if (userPermission == null) return false;

            _context.UserPermissions.Remove(userPermission);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
