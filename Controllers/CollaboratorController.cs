using CredipathAPI.DTOs;
using CredipathAPI.Model;
using CredipathAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CredipathAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class CollaboratorController : ControllerBase
    {
        private readonly CollaboratorService _collaboratorService;
        private readonly ILogger<CollaboratorController> _logger;

        public CollaboratorController(CollaboratorService collaboratorService, ILogger<CollaboratorController> logger)
        {
            _collaboratorService = collaboratorService;
            _logger = logger;
        }

        // GET: api/Collaborator
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CollaboratorResponseDTO>>>> GetCollaborators()
        {
            try
            {
                var collaborators = await _collaboratorService.GetAllCollaboratorsAsync();

                return Ok(new ApiResponse<IEnumerable<CollaboratorResponseDTO>>
                {
                    Success = true,
                    Message = "Colaboradores obtenidos con éxito",
                    Data = collaborators
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los colaboradores");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al obtener los colaboradores: " + ex.Message,
                    Data = null
                });
            }
        }

        // GET: api/Collaborator/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CollaboratorResponseDTO>>> GetCollaborator(int id)
        {
            try
            {
                var collaborator = await _collaboratorService.GetCollaboratorByIdAsync(id);

                return Ok(new ApiResponse<CollaboratorResponseDTO>
                {
                    Success = true,
                    Message = "Colaborador obtenido con éxito",
                    Data = collaborator
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el colaborador con ID: {id}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al obtener el colaborador: " + ex.Message,
                    Data = null
                });
            }
        }

        // POST: api/Collaborator
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CollaboratorResponseDTO>>> CreateCollaborator(CollaboratorCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Datos de colaborador inválidos",
                        Data = ModelState
                    });
                }

                // Obtener el ID del usuario que está realizando la operación (el admin autenticado)
                var creatorUserId = GetCurrentUserId();
                
                var createdCollaborator = await _collaboratorService.CreateCollaboratorAsync(dto, creatorUserId);

                return CreatedAtAction(
                    nameof(GetCollaborator),
                    new { id = createdCollaborator.Id },
                    new ApiResponse<CollaboratorResponseDTO>
                    {
                        Success = true,
                        Message = "Colaborador creado con éxito",
                        Data = createdCollaborator
                    });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el colaborador");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al crear el colaborador: " + ex.Message,
                    Data = null
                });
            }
        }

        // PUT: api/Collaborator/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CollaboratorResponseDTO>>> UpdateCollaborator(int id, CollaboratorUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Datos de colaborador inválidos",
                        Data = ModelState
                    });
                }

                var updatedCollaborator = await _collaboratorService.UpdateCollaboratorAsync(id, dto);

                return Ok(new ApiResponse<CollaboratorResponseDTO>
                {
                    Success = true,
                    Message = "Colaborador actualizado con éxito",
                    Data = updatedCollaborator
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el colaborador con ID: {id}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al actualizar el colaborador: " + ex.Message,
                    Data = null
                });
            }
        }

        // DELETE: api/Collaborator/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCollaborator(int id)
        {
            try
            {
                await _collaboratorService.DeleteCollaboratorAsync(id);
                
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Colaborador eliminado con éxito",
                    Data = null
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el colaborador con ID: {id}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al eliminar el colaborador: " + ex.Message,
                    Data = null
                });
            }
        }
        
        // Método auxiliar para obtener el ID del usuario autenticado
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            
            throw new InvalidOperationException("No se pudo obtener el ID del usuario autenticado");
        }
    }
}
