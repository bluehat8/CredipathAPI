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
    public class UserService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.username == username);
        }

        public bool VerifyPassword(User user, string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            return passwordHasher.VerifyHashedPassword(user, user.password, password) != PasswordVerificationResult.Failed;
        }

        public string GenerateJwtToken(User user)
        {
            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("user", user.username),
                new Claim("usertype", user.UserType.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            var existingEmailUser = await _context.Users.FirstOrDefaultAsync(u => u.email == user.email);
            if (existingEmailUser != null)
            {
                throw new Exception("El email ya está en uso.");
            }

            var existingUsernameUser = await _context.Users.FirstOrDefaultAsync(u => u.username == user.username);
            if (existingUsernameUser != null)
            {
                throw new Exception("El username ya está en uso.");
            }

            var passwordHasher = new PasswordHasher<User>();
            user.password = passwordHasher.HashPassword(user, password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> RegisterCollaboratorAsync(UserDTO dto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.email == dto.Email);
            if (existingUser != null)
            {
                throw new Exception("El email ya está en uso.");
            }

            var existingUsernameUser = await _context.Users.FirstOrDefaultAsync(u => u.username == dto.Username);
            if (existingUsernameUser != null)
            {
                throw new Exception("El username ya está en uso.");
            }

            var passwordHasher = new PasswordHasher<User>();

            var user = new User
            {
                name = dto.Username,
                username = dto.Username,
                email = dto.Email,
                UserType = Constants.UserType.collaborator,
                note = dto.Note,
                address = dto.Address,
                code = dto.Code,
            };

            user.password = passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Validar permisos
            if (dto.PermissionIds?.Any(id => id > 0) == true)
            {
                var existingPermissions = await _context.Permissions
                    .Where(p => dto.PermissionIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                foreach (var permissionId in existingPermissions)
                {
                    var userPermission = new UserPermission
                    {
                        UserId = user.Id,
                        PermissionId = permissionId
                    };
                    _context.UserPermissions.Add(userPermission);
                }
            }

            // Validar rutas
            if (dto.RouteIds?.Any(id => id > 0) == true)
            {
                var existingRoutes = await _context.Routes
                    .Where(r => dto.RouteIds.Contains(r.Id))
                    .Select(r => r.Id)
                    .ToListAsync();

                foreach (var routeId in existingRoutes)
                {
                    var userRoute = new UserRoute
                    {
                        UserId = user.Id,
                        RouteId = routeId
                    };
                    _context.userRoutes.Add(userRoute);
                }
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
