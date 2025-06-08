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
using CredipathAPI.DTOs;
using CredipathAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;

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
        [HttpGet("getRoutes")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetRoutes(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var response = await _routeService.GetPaginatedRoutesAsync(page, pageSize);
            return Ok(new 
            { 
                success = true,
                data = response
            });
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

            if (!string.IsNullOrWhiteSpace(routeDto.name))
            {
                existingRoute.route_name = routeDto.name;
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

        // POST: api/Route
        [HttpPost("addRoutes")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<RouteResponseDTO>> PostRoute(RouteDTO routeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Datos de entrada no válidos",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var createdRoute = await _routeService.CreateRouteAsync(routeDto);
                
                return CreatedAtAction(
                    nameof(GetRoute), 
                    new { id = createdRoute.Id }, 
                    new 
                    { 
                        success = true,
                        data = createdRoute,
                        message = "Ruta creada exitosamente"
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = ex.Message 
                });
            }
            catch (Exception ex)
            {
                // En producción, registrar el error
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Ocurrió un error al procesar la solicitud" 
                });
            }
        }
    }
}
