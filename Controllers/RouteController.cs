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



        // PUT: api/Route/updateRoute/5
        [HttpPut("updateRoute/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] UpdateRouteDTO updateDto)
        {
            try
            {            
                // Call the service to update the route
                var result = await _routeService.UpdateRouteAsync(updateDto, id);

                // Return appropriate response based on service result
                if (!result.Success)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.Message 
                    });
                }


                return Ok(new { 
                    success = true,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, new { 
                    success = false, 
                    message = "Error interno del servidor al actualizar la ruta" 
                });
            }
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
                
                return Ok(new 
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
