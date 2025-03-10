using CredipathAPI.DTOs;
using CredipathAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CredipathAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class UserRoutesController: ControllerBase
    {
        private readonly UserRouteService _userRouteService;

        public UserRoutesController(UserRouteService userRouteService)
        {
            _userRouteService = userRouteService;
        }


        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUserRoutes()
        {
            try
            {
                var userRoutes = await _userRouteService.GetUserRoutesAsync();
                return Ok(userRoutes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ocurrió un error al obtener las rutas de los usuarios.", Details = ex.Message });
            }
        }


        [HttpPost("AssignRoute")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AssignRoute([FromBody] AssignedRouteDTO assignRouteDTO)
        {
            try
            {
                var result = await _userRouteService.AssignRouteAsync(assignRouteDTO);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ocurrió un error al asignar la ruta.", Details = ex.Message });
            }
        }

    }
}
