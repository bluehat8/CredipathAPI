using CredipathAPI.Data;
using CredipathAPI.DTOs;
using CredipathAPI.Services;
using CredipathAPI.Helpers;
using CredipathAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Route = CredipathAPI.Model.Route;
using System.Net;

namespace CredipathAPI.Services
{
    public class RouteServices
    {
        private readonly DataContext _context;

        public RouteServices(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Model.Route>> GetAllRoutesAsync()
        {
            return await _context.Routes
                .Where(s => s.Active)
                .ToListAsync();
        }
        
        public async Task<PagedResponse<RouteResponseDTO>> GetRoutesByCreatorAsync(int createdById, int page = 1, int pageSize = 10)
        {
            var query = _context.Routes
                .Where(r => r.Active && r.CreatedById == createdById)
                .OrderByDescending(r => r.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RouteResponseDTO
                {
                    Id = r.Id,
                    Name = r.route_name,
                    Description = r.description,
                    District = r.District,
                    Location = r.Location,
                    CreatedAt = r.CreatedAt,
                    Status = r.Active ? "active" : "inactive"
                })
                .ToListAsync();

            return new PagedResponse<RouteResponseDTO>
            {
                Items = items,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = totalItems
                }
            };
        }

        public async Task<RouteResponseDTO> CreateRouteAsync(RouteDTO routeDto, int createdById)
        {
            // Verificar que el usuario creador exista
            var userExists = await _context.Users.AnyAsync(u => u.Id == createdById);
            if (!userExists)
            {
                throw new InvalidOperationException("El usuario especificado no existe.");
            }

            // Verificar si ya existe una ruta con el mismo nombre
            var existingRoute = await _context.Routes
                .FirstOrDefaultAsync(r => r.route_name.ToLower() == routeDto.name.ToLower());

            if (existingRoute != null)
            {
                throw new InvalidOperationException("Ya existe una ruta con este nombre.");
            }


            // Crear nueva ruta
            Route newRoute = new Route
            {
                route_name = routeDto.name,
                description = routeDto.description,
                District = routeDto.District,
                Location = routeDto.Location,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedById = createdById
            };

            _context.Routes.Add(newRoute);
            await _context.SaveChangesAsync();

            // Mapear a DTO para la respuesta
            return new RouteResponseDTO
            {
                Id = newRoute.Id,
                Name = newRoute.route_name,
                Description = newRoute.description,
                District = newRoute.District,
                Location = newRoute.Location,
                CreatedAt = newRoute.CreatedAt,
                Status = "active"
            };
        }

        public async Task<RouteResponseDTO> GetRouteByIdAsync(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null) return null;

            return new RouteResponseDTO
            {
                Id = route.Id,
                Name = route.route_name,
                Description = route.description,
                District = route.District,
                Location = route.Location,
                Status = route.Active ? "active" : "inactive"
            };
        }

        public async Task<ApiResponse<object>> UpdateRouteAsync(UpdateRouteDTO updateDto, int routeId)
        {
            try
            {
                // Find the existing route
                var existingRoute = await _context.Routes.FindAsync(routeId);
                if (existingRoute == null)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Ruta no encontrada"
                    };
                }

                // Check if another route with the same name exists (case insensitive)
                var duplicateRoute = await _context.Routes
                    .FirstOrDefaultAsync(r => 
                        r.Id != routeId && 
                        r.route_name.ToLower() == updateDto.Name.ToLower());

                if (duplicateRoute != null)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Ya existe una ruta con este nombre"
                    };
                }


                // Update the route properties
                existingRoute.route_name = updateDto.Name;
                existingRoute.description = updateDto.Description;
                existingRoute.District = updateDto.District;
                existingRoute.Location = updateDto.Location;
                existingRoute.Active = updateDto.Status.ToLower() == "active";
                existingRoute.UpdatedAt = DateTime.UtcNow;

                // Save changes
                await _context.SaveChangesAsync();

                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Ruta actualizada correctamente"
                };
            }
            catch (Exception ex)
            {
                // Log the error
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al actualizar la ruta: " + ex.Message
                };
            }
        }

        public async Task<PagedResponse<RouteResponseDTO>> GetPaginatedRoutesAsync(int page = 1, int pageSize = 10)
        {
            var query = _context.Routes
                //.Where(r => r.Active)
                .Select(r => new RouteResponseDTO
                {
                    Id = r.Id,
                    Name = r.route_name,
                    Description = r.description,
                    Status = r.Active ? "active" : "inactive",
                    District = r.District,
                    Location = r.Location,
                    ClientsCount = r.Clients != null ? r.Clients.Count(c => c.Active) : 0,
                    CollaboratorsCount = r.UserRoutes != null ? r.UserRoutes.Count(ur => ur.User != null && ur.User.Active) : 0,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    LastVisit = r.UpdatedAt
                });

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<RouteResponseDTO>
            {
                Items = items,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = totalItems
                }
            };
        }

      

        public async Task UpdateRouteAsync(Model.Route route)
        {
            _context.Routes.Update(route);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRouteAsync(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route != null)
            {
                route.Active = false; 
                _context.Routes.Update(route); 
                await _context.SaveChangesAsync();  
            }
        }

        public bool RouteExists(int id)
        {
            return _context.Routes.Any(e => e.Id == id);
        }

    }
}
