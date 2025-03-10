using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CredipathAPI.Data;
using CredipathAPI.Model;
using CredipathAPI.Helpers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Diagnostics.SymbolStore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CredipathAPI.Services;
using CredipathAPI.DTOs;

namespace CredipathAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : Controller
    {
        private readonly RouteServices _routeService;
        public RouteController(RouteServices routeService)
        {
            _routeService = routeService;
        }

        // GET: api/Route
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Model.Route>>> GetRoutes()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        // GET: api/Route/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Model.Route>> GetRoute(int id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);

            if (route == null)
            {
                return NotFound();
            }

            return Ok(route);
        }


        // POST: Route/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Model.Route>> PostRoute(RouteDTO routeDto)
        {
            Model.Route newRoute = new Model.Route
            {
                route_name = routeDto.route_name,
                description = routeDto.description,
                Clients = null  
            };

            await _routeService.CreateRouteAsync(newRoute);
            return CreatedAtAction(nameof(GetRoute), new { id = newRoute.Id }, newRoute);
        }



        // POST: Route/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> PutRoute(int id, RouteDTO routeDto)
        {
            var existingRoute = await _routeService.GetRouteByIdAsync(id);

            if (existingRoute == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(routeDto.route_name))
            {
                existingRoute.route_name = routeDto.route_name;
            }

            if (!string.IsNullOrWhiteSpace(routeDto.description))
            {
                existingRoute.description = routeDto.description;
            }

            existingRoute.UpdatedAt = DateTime.UtcNow;

            await _routeService.UpdateRouteAsync(existingRoute);

            return NoContent();
        }



        // DELETE: api/Route/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteRoute(int id)
        {

            if (!_routeService.RouteExists(id))
            {
                return NotFound();
            }

            await _routeService.DeleteRouteAsync(id);  
            return NoContent();
        }
    }
}
