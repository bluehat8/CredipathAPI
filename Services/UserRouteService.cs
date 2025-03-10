using CredipathAPI.Data;
using CredipathAPI.DTOs;
using CredipathAPI.Migrations;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace CredipathAPI.Services
{
    public class UserRouteService
    {
        private readonly DataContext _context;

        public UserRouteService(DataContext context)
        {
            _context = context;
        }

        public async Task<UserRouteDTO> AssignRouteAsync(AssignedRouteDTO assignRouteDTO)
        {
            var user = await _context.Users.FindAsync(assignRouteDTO.UserId);
            if (user == null)
            {
                throw new ArgumentException("El usuario especificado no existe.");
            }

            var route = await _context.Routes.FindAsync(assignRouteDTO.RouteId);
            if (route == null)
            {
                throw new ArgumentException("La ruta especificada no existe.");
            }

            // Validar si la asignación ya existe
            var existingAssignment = await _context.userRoutes
                .AnyAsync(ur => ur.UserId == assignRouteDTO.UserId && ur.RouteId == assignRouteDTO.RouteId);
            if (existingAssignment)
            {
                throw new InvalidOperationException("Esta ruta ya está asignada al usuario.");
            }

            var userRoute = new UserRoute
            {
                UserId= assignRouteDTO.UserId,
                RouteId = assignRouteDTO.RouteId
            };

            _context.userRoutes.Add(userRoute);
            await _context.SaveChangesAsync();

            return new UserRouteDTO
            {
                UserId = user.Id,
                UserName = user.username,
                RouteId = route.Id,
                RouteName = route.route_name
            };
        }


        public async Task<List<UserRouteDTO>> GetUserRoutesAsync()
        {
            var userRoutes = await _context.userRoutes
                .Include(ur => ur.User)
                .Include(ur => ur.Route) 
                .Select(ur => new UserRouteDTO
                {
                    UserId = ur.User.Id,
                    UserName = ur.User.username,
                    RouteId = ur.Route.Id,
                    RouteName = ur.Route.route_name,
                    RouteDescription = ur.Route.description
                })
                .ToListAsync();

            return userRoutes;
        }

    }

}
