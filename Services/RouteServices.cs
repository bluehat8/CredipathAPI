using CredipathAPI.Data;
using CredipathAPI.DTOs;
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
    public class RouteServices
    {
        private readonly DataContext _context;

        public RouteServices(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Model.Route>> GetAllRoutesAsync()
        {
            return await _context.Routes.Where(s =>s.Active).ToListAsync();
        }

        public async Task<PagedResponse<RouteResponseDTO>> GetPaginatedRoutesAsync(int page = 1, int pageSize = 10)
        {
            var query = _context.Routes
                .Where(r => r.Active)
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

        public async Task<Model.Route> GetRouteByIdAsync(int id)
        {
            return await _context.Routes.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task CreateRouteAsync(Model.Route route)
        {
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
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
