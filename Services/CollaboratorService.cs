using CredipathAPI.Data;
using CredipathAPI.DTOs;
using CredipathAPI.Helpers;
using CredipathAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
using System.Security.Claims;
using static CredipathAPI.Constants;


namespace CredipathAPI.Services
{
    public class CollaboratorService
    {
        private readonly DataContext _context;
        private readonly ILogger<CollaboratorService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CollaboratorService(DataContext context, ILogger<CollaboratorService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        public async Task<IEnumerable<CollaboratorResponseDTO>> GetAllCollaboratorsAsync()
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "id");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                {
                    _logger.LogWarning("No se pudo obtener el ID del usuario autenticado");
                    throw new UnauthorizedAccessException("Usuario no autenticado");
                }

                _logger.LogInformation($"Obteniendo colaboradores para el usuario con ID: {currentUserId}");
                var collaborators = await _context.Collaborators
                    .Where(c => c.CreatedById == currentUserId)
                    .Include(c => c.User)
                    .Include(c => c.CreatedBy)
                    .ToListAsync();
                
                var result = new List<CollaboratorResponseDTO>();
                foreach (var collaborator in collaborators)
                {
                    result.Add(await GetMappedCollaboratorResponseAsync(collaborator));
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los colaboradores");
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
                
                return await GetMappedCollaboratorResponseAsync(collaborator);
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

            // Usar la estrategia de ejecución con reintentos
            var strategy = _context.Database.CreateExecutionStrategy();
            
            return await strategy.ExecuteAsync(async () => 
            {
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
                        username = dto.Email,
                        email = dto.Email,
                        address = dto.Address,
                        UserType = UserType.collaborator
                    };
                    
                    user.password = passwordHasher.HashPassword(user, dto.Password);
                    
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync(); 

                    // 2. Crear el colaborador asociado
                    var collaborator = new Collaborator
                    {
                        Identifier = dto.Identifier,
                        Phone = dto.Phone,
                        Mobile = dto.Mobile,
                        UserId = user.Id,
                        CreatedById = creatorUserId
                    };

                    _context.Collaborators.Add(collaborator);
                    await _context.SaveChangesAsync();
                    
                    // 3. Asignar permisos al usuario creado
                    if (dto.PermissionIds != null && dto.PermissionIds.Count > 0)
                    {
                        foreach (var permissionId in dto.PermissionIds)
                        {
                            if (await _context.Permissions.AnyAsync(p => p.Id == permissionId))
                            {
                                var userPermission = new UserPermission
                                {
                                    UserId = user.Id,
                                    PermissionId = permissionId
                                };
                                _context.UserPermissions.Add(userPermission);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                    
                    await transaction.CommitAsync();

                    // Cargar los datos relacionados para el mapeo
                    await _context.Entry(collaborator).Reference(c => c.User).LoadAsync();
                    await _context.Entry(collaborator).Reference(c => c.CreatedBy).LoadAsync();

                    _logger.LogInformation($"Colaborador creado con ID: {collaborator.Id}, asociado al usuario ID: {user.Id}");
                    return await GetMappedCollaboratorResponseAsync(collaborator);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<CollaboratorResponseDTO> UpdateCollaboratorAsync(int id, CollaboratorUpdateDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Usar la estrategia de ejecución con reintentos
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
            
                try
                {
                    // Mover todas las operaciones de base de datos dentro de la transacción
                    var collaborator = await _context.Collaborators
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.Id == id);


                    if (collaborator == null)
                    {
                        _logger.LogWarning($"Intento de actualizar colaborador inexistente con ID: {id}");
                        throw new KeyNotFoundException($"No se encontró colaborador con ID: {id}");
                    }

                    // Verificar duplicados de identificador
                    if (!string.IsNullOrEmpty(dto.Identifier))
                    {
                        bool identifierExists = await _context.Collaborators
                            .AnyAsync(c => c.Identifier == dto.Identifier && c.Id != id);
                            
                        if (identifierExists)
                        {
                            _logger.LogWarning($"Intento de actualizar colaborador con identificador duplicado: {dto.Identifier}");
                            throw new InvalidOperationException($"Ya existe otro colaborador con el identificador: {dto.Identifier}");
                        }
                        collaborator.Identifier = dto.Identifier;
                    }

                    // Actualizar campos básicos
                    Helper.UpdateIfNotEmpty(dto.Phone, value => collaborator.Phone = value);
                    Helper.UpdateIfNotEmpty(dto.Mobile, value => collaborator.Mobile = value);

                    // Actualizar datos de usuario si existen
                    if (collaborator.User != null)
                    {
                        Helper.UpdateIfNotEmpty(dto.Name, value => collaborator.User.name = value);
                        Helper.UpdateIfNotEmpty(dto.Email, value => collaborator.User.email = value);
                        Helper.UpdateIfNotEmpty(dto.Address, value => collaborator.User.address = value);
                        
                        if (!string.IsNullOrEmpty(dto.Password))
                        {
                            var passwordHasher = new PasswordHasher<User>();
                            collaborator.User.password = passwordHasher.HashPassword(collaborator.User, dto.Password);
                        }
                    }

                    // Actualizar permisos si se proporcionaron
                    if (dto.Permissions != null)
                    {
                        // Eliminar permisos actuales
                        await _context.UserPermissions
                            .Where(up => up.UserId == collaborator.UserId)
                            .ExecuteDeleteAsync();

                        // Obtener solo los IDs de permisos válidos
                        var validPermissionIds = await _context.Permissions
                            .Where(p => dto.Permissions.Contains(p.Id))
                            .Select(p => p.Id)
                            .ToListAsync();

                        // Agregar los nuevos permisos
                        foreach (var permissionId in validPermissionIds)
                        {
                            _context.UserPermissions.Add(new UserPermission
                            {
                                UserId = collaborator.UserId,
                                PermissionId = permissionId
                            });
                        }

                    }

                    // Actualizar marca de tiempo
                    collaborator.UpdatedAt = DateTime.UtcNow;


                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    await _context.Entry(collaborator).Reference(c => c.CreatedBy).LoadAsync();
                    
                    _logger.LogInformation($"Colaborador actualizado con ID: {id}");
                    return await GetMappedCollaboratorResponseAsync(collaborator);
                }
                catch (Exception ex) when (ex is not (KeyNotFoundException or InvalidOperationException))
                {
                    _logger.LogError(ex, $"Error al actualizar colaborador con ID: {id}");
                    throw;
                }
            });
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

        // Función para convertir la estructura anidada de permisos a IDs de permisos en el sistema
        private async Task<List<int>> GetPermissionIdsFromNestedStructureAsync(NestedPermissionsDTO permissions)
        {
            var result = new List<int>();
            
            if (permissions == null)
                return result;
                
            try
            {
                // 1. Permisos de préstamos (Loan)
                if (permissions.Loan != null)
                {
                    // Buscar el ID del permiso para "Loan Add"
                    if (permissions.Loan.Add)
                    {
                        var loanAddPermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "Prestamos" && p.Action == "Agregar");
                            
                        if (loanAddPermission != null)
                            result.Add(loanAddPermission.Id);
                    }
                    
                    // Buscar el ID del permiso para "Loan Edit"
                    if (permissions.Loan.Edit)
                    {
                        var loanEditPermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "Prestamos" && p.Action == "Editar");
                            
                        if (loanEditPermission != null)
                            result.Add(loanEditPermission.Id);
                    }
                    
                    // Buscar el ID del permiso para "Loan Delete"
                    if (permissions.Loan.Delete)
                    {
                        var loanDeletePermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "Prestamos" && p.Action == "Eliminar");
                            
                        if (loanDeletePermission != null)
                            result.Add(loanDeletePermission.Id);
                    }
                }
                
                // 2. Permisos de pagos (Payment)
                if (permissions.Payment != null)
                {
                    // Buscar el ID del permiso para "Payment Add"
                    if (permissions.Payment.Add)
                    {
                        var paymentAddPermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "Pagos" && p.Action == "Agregar");
                            
                        if (paymentAddPermission != null)
                            result.Add(paymentAddPermission.Id);
                    }
                    
                    // Buscar el ID del permiso para "Payment Edit"
                    if (permissions.Payment.Edit)
                    {
                        var paymentEditPermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "Pagos" && p.Action == "Editar");
                            
                        if (paymentEditPermission != null)
                            result.Add(paymentEditPermission.Id);
                    }
                    
                    // Buscar el ID del permiso para "Payment Delete"
                    if (permissions.Payment.Delete)
                    {
                        var paymentDeletePermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "Pagos" && p.Action == "Eliminar");
                            
                        if (paymentDeletePermission != null)
                            result.Add(paymentDeletePermission.Id);
                    }
                }
                
                // 3. Permisos de módulos (Modules)
                if (permissions.Modules != null)
                {
                    // Módulo de Colaboradores
                    if (permissions.Modules.Collaborators)
                    {
                        var collaboratorsPermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "Colaboradores" && p.Action == "Ver");
                            
                        if (collaboratorsPermission != null)
                            result.Add(collaboratorsPermission.Id);
                    }
                    
                    // Módulo de Pagos vencidos
                    if (permissions.Modules.OverduePayments)
                    {
                        var overduePaymentsPermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "PagosVencidos" && p.Action == "Ver");
                            
                        if (overduePaymentsPermission != null)
                            result.Add(overduePaymentsPermission.Id);
                    }
                    
                    // Módulo de Pagos próximos
                    if (permissions.Modules.UpcomingPayments)
                    {
                        var upcomingPaymentsPermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "PagosProximos" && p.Action == "Ver");
                            
                        if (upcomingPaymentsPermission != null)
                            result.Add(upcomingPaymentsPermission.Id);
                    }
                    
                    // Módulo de Pago de préstamos
                    if (permissions.Modules.LoanPayment)
                    {
                        var loanPaymentPermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "PagoPrestamos" && p.Action == "Ver");
                            
                        if (loanPaymentPermission != null)
                            result.Add(loanPaymentPermission.Id);
                    }
                    
                    // Módulo de Reportes
                    if (permissions.Modules.Report)
                    {
                        var reportPermission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Module == "Reportes" && p.Action == "Ver");
                            
                        if (reportPermission != null)
                            result.Add(reportPermission.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al convertir estructura anidada de permisos a IDs");
            }
            
            return result;
        }
        
        // Método para mapear de Collaborator a CollaboratorResponseDTO incluyendo permisos
        public async Task<CollaboratorResponseDTO> GetMappedCollaboratorResponseAsync(Collaborator collaborator)
        {
            if (collaborator == null)
                return null;
            
            // Obtener los permisos del usuario
            var userPermissions = await _context.UserPermissions
                .Include(up => up.Permission)
                .Where(up => up.UserId == collaborator.UserId)
                .ToListAsync();
                
            var permissionDtos = userPermissions
                .Select(up => new DTOs.PermissionDTO {
                    Id = up.Permission.Id,
                    Module = up.Permission.Module,
                    Action = up.Permission.Action
                }).ToList();
                
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
                Permissions = permissionDtos
            };
        }
    }
}
