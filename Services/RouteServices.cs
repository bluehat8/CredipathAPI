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
