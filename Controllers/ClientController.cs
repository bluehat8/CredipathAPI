using CredipathAPI.DTOs;
using CredipathAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CredipathAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientService clientService, ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene una lista paginada de clientes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ClientResponseDTO>>>> GetClients(
            [FromQuery] ClientQueryParams queryParams)
        {
            try
            {
                var result = await _clientService.GetClientsAsync(queryParams);
                return Ok(ApiResponse<PaginatedResponse<ClientResponseDTO>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de clientes");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Ocurrió un error al procesar la solicitud"));
            }
        }

        /// <summary>
        /// Obtiene un cliente por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ClientResponseDTO>>> GetClient(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                return Ok(ApiResponse<ClientResponseDTO>.SuccessResponse(client));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el cliente con ID {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Ocurrió un error al procesar la solicitud"));
            }
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<ClientResponseDTO>>> CreateClient(CreateClientDTO clientDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var createdClient = await _clientService.CreateClientAsync(clientDto, userId);
                return CreatedAtAction(nameof(GetClient), new { id = createdClient.Id }, 
                    ApiResponse<ClientResponseDTO>.SuccessResponse(createdClient, "Cliente creado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el cliente");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Ocurrió un error al crear el cliente"));
            }
        }

        /// <summary>
        /// Actualiza un cliente existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, UpdateClientDTO clientDto)
        {
            try
            {
                await _clientService.UpdateClientAsync(id, clientDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el cliente con ID {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Ocurrió un error al actualizar el cliente"));
            }
        }

        /// <summary>
        /// Elimina un cliente (eliminación lógica)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                await _clientService.DeleteClientAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el cliente con ID {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Ocurrió un error al eliminar el cliente"));
            }
        }
    }
}