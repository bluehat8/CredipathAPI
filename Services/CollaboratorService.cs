using CredipathAPI.Data;
using CredipathAPI.DTOs;
using CredipathAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static CredipathAPI.Constants;

namespace CredipathAPI.Services
{
    public class CollaboratorService
    {
        private readonly DataContext _context;
        private readonly ILogger<CollaboratorService> _logger;

        public CollaboratorService(DataContext context, ILogger<CollaboratorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CollaboratorResponseDTO>> GetAllCollaboratorsAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los colaboradores");
                var collaborators = await _context.Collaborators
                    .Include(c => c.User)
                    .Include(c => c.CreatedBy)
                    .ToListAsync();
                    
                return collaborators.Select(c => MapToResponseDTO(c));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los colaboradores");
                throw;
            }
        }

        public async Task<CollaboratorResponseDTO> GetCollaboratorByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Obteniendo colaborador con ID: {id}");
                var collaborator = await _context.Collaborators
                    .Include(c => c.User)
                    .Include(c => c.CreatedBy)
                    .FirstOrDefaultAsync(c => c.Id == id);
                
                if (collaborator == null)
                {
                    _logger.LogWarning($"Colaborador con ID: {id} no encontrado");
                    throw new KeyNotFoundException($"No se encontró colaborador con ID: {id}");
                }
                
                return MapToResponseDTO(collaborator);
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                _logger.LogError(ex, $"Error al obtener colaborador con ID: {id}");
                throw;
            }
        }

        public async Task<CollaboratorResponseDTO> CreateCollaboratorAsync(CollaboratorCreateDTO dto, int creatorUserId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Verificar si ya existe un usuario con el mismo email
                if (await _context.Users.AnyAsync(u => u.email == dto.Email))
                {
                    _logger.LogWarning($"Intento de crear colaborador con email duplicado: {dto.Email}");
                    throw new InvalidOperationException($"Ya existe un usuario con el email: {dto.Email}");
                }

                // Verificar si ya existe un usuario con el mismo username (usaremos el email como username)
                if (await _context.Users.AnyAsync(u => u.username == dto.Email))
                {
                    _logger.LogWarning($"Intento de crear colaborador con username duplicado: {dto.Email}");
                    throw new InvalidOperationException($"Ya existe un usuario con el username: {dto.Email}");
                }
                
                // Verificar si ya existe un colaborador con el mismo identificador
                if (await _context.Collaborators.AnyAsync(c => c.Identifier == dto.Identifier))
                {
                    _logger.LogWarning($"Intento de crear colaborador con identificador duplicado: {dto.Identifier}");
                    throw new InvalidOperationException($"Ya existe un colaborador con el identificador: {dto.Identifier}");
                }

                // 1. Crear el usuario
                var passwordHasher = new PasswordHasher<User>();
                
                var user = new User
                {
                    name = dto.Name,
                    username = dto.Email, // Usar email como username
                    email = dto.Email,
                    address = dto.Address,
                    UserType = UserType.collaborator
                };
                
                user.password = passwordHasher.HashPassword(user, dto.Password);
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // Guardar para obtener el ID

                // 2. Crear el colaborador asociado con los permisos del DTO
                var collaborator = new Collaborator
                {
                    Identifier = dto.Identifier,
                    Phone = dto.Phone,
                    Mobile = dto.Mobile,
                    UserId = user.Id,
                    CreatedById = creatorUserId,
                    Loan = dto.Permissions?.Loan ?? new LoanPermissions(),
                    Payment = dto.Permissions?.Payment ?? new PaymentPermissions(),
                    Modules = dto.Permissions?.Modules ?? new ModulePermissions()
                };

                _context.Collaborators.Add(collaborator);
                await _context.SaveChangesAsync();
                
                // Confirmar la transacción
                await transaction.CommitAsync();

                // Cargar los datos relacionados para el mapeo
                await _context.Entry(collaborator).Reference(c => c.User).LoadAsync();
                await _context.Entry(collaborator).Reference(c => c.CreatedBy).LoadAsync();

                _logger.LogInformation($"Colaborador creado con ID: {collaborator.Id}, asociado al usuario ID: {user.Id}");
                return MapToResponseDTO(collaborator);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                
                if (ex is InvalidOperationException)
                    throw;
                    
                _logger.LogError(ex, "Error al crear colaborador");
                throw;
            }
        }

        public async Task<CollaboratorResponseDTO> UpdateCollaboratorAsync(int id, CollaboratorUpdateDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Obtener el colaborador con sus relaciones
                var collaborator = await _context.Collaborators
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == id);
                    
                if (collaborator == null)
                {
                    _logger.LogWarning($"Intento de actualizar colaborador inexistente con ID: {id}");
                    throw new KeyNotFoundException($"No se encontró colaborador con ID: {id}");
                }

                // Verificar si el identificador ya está en uso por otro colaborador
                if (!string.IsNullOrEmpty(dto.Identifier) && 
                    await _context.Collaborators.AnyAsync(c => c.Identifier == dto.Identifier && c.Id != id))
                {
                    _logger.LogWarning($"Intento de actualizar colaborador con identificador duplicado: {dto.Identifier}");
                    throw new InvalidOperationException($"Ya existe otro colaborador con el identificador: {dto.Identifier}");
                }

                // Actualizar propiedades del colaborador
                if (!string.IsNullOrEmpty(dto.Identifier))
                    collaborator.Identifier = dto.Identifier;
                    
                if (!string.IsNullOrEmpty(dto.Phone))
                    collaborator.Phone = dto.Phone;
                    
                if (!string.IsNullOrEmpty(dto.Mobile))
                    collaborator.Mobile = dto.Mobile;
                
                // Actualizar permisos
                if (dto.Loan != null)
                    collaborator.Loan = dto.Loan;
                    
                if (dto.Payment != null)
                    collaborator.Payment = dto.Payment;
                    
                if (dto.Modules != null)
                    collaborator.Modules = dto.Modules;
                    
                collaborator.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                // Cargar el usuario que creó el colaborador para el mapeo
                await _context.Entry(collaborator).Reference(c => c.CreatedBy).LoadAsync();
                
                _logger.LogInformation($"Colaborador actualizado con ID: {id}");
                return MapToResponseDTO(collaborator);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                
                if (ex is KeyNotFoundException || ex is InvalidOperationException)
                    throw;
                    
                _logger.LogError(ex, $"Error al actualizar colaborador con ID: {id}");
                throw;
            }
        }

        public async Task DeleteCollaboratorAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var collaborator = await _context.Collaborators
                    .FirstOrDefaultAsync(c => c.Id == id);
                    
                if (collaborator == null)
                {
                    _logger.LogWarning($"Intento de eliminar colaborador inexistente con ID: {id}");
                    throw new KeyNotFoundException($"No se encontró colaborador con ID: {id}");
                }
                                
                _context.Collaborators.Remove(collaborator);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                _logger.LogInformation($"Colaborador eliminado con ID: {id}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                
                if (ex is KeyNotFoundException)
                    throw;
                    
                _logger.LogError(ex, $"Error al eliminar colaborador con ID: {id}");
                throw;
            }
        }

        // Método para mapear de Collaborator a CollaboratorResponseDTO
        public CollaboratorResponseDTO MapToResponseDTO(Collaborator collaborator)
        {
            if (collaborator == null)
                return null;
                
            return new CollaboratorResponseDTO
            {
                Id = collaborator.Id,
                Identifier = collaborator.Identifier,
                Phone = collaborator.Phone,
                Mobile = collaborator.Mobile,
                
                // Datos del usuario asociado
                UserId = collaborator.UserId,
                Name = collaborator.User?.name,
                Username = collaborator.User?.username,
                Email = collaborator.User?.email,
                Address = collaborator.User?.address,
                
                // Información de creación
                CreatedById = collaborator.CreatedById,
                CreatedByName = collaborator.CreatedBy?.name,
                CreatedAt = collaborator.CreatedAt,
                
                // Permisos
                Loan = collaborator.Loan,
                Payment = collaborator.Payment,
                Modules = collaborator.Modules
            };
        }
    }
}
