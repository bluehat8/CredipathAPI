using CredipathAPI.Data;
using CredipathAPI.DTOs;
using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CredipathAPI.Services
{

    public class ClientService : IClientService
    {
        private readonly DataContext _context;
        private readonly ILogger<ClientService> _logger;

        public ClientService(DataContext context, ILogger<ClientService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ClientResponseDTO>> GetClientsAsync(ClientQueryParams queryParams)
        {
            try
            {
                var query = _context.Clients
                    .Include(c => c.Route)
                    .Where(c => c.Active)
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
                {
                    var searchTerm = queryParams.SearchTerm.ToLower();
                    query = query.Where(c => 
                        (c.name != null && c.name.ToLower().Contains(searchTerm)) ||
                        (c.email != null && c.email.ToLower().Contains(searchTerm)) ||
                        (c.phone != null && c.phone.Contains(searchTerm)));
                }

                if (queryParams.RouteId.HasValue)
                {
                    query = query.Where(c => c.RouteId == queryParams.RouteId.Value);
                }

                // Contar total antes de paginar
                var totalItems = await query.CountAsync();

                // Aplicar paginación
                var clients = await query
                    .OrderBy(c => c.name)
                    .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .Select(c => MapToClientResponse(c))
                    .ToListAsync();

                return new PaginatedResponse<ClientResponseDTO>
                {
                    Items = clients,
                    TotalCount = totalItems,
                    PageNumber = queryParams.PageNumber,
                    PageSize = queryParams.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de clientes");
                throw;
            }
        }

        public async Task<ClientResponseDTO> GetClientByIdAsync(int id)
        {
            try
            {
                var client = await _context.Clients
                    .Include(c => c.Route)
                    .FirstOrDefaultAsync(c => c.Id == id && c.Active);

                if (client == null)
                {
                    throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");
                }

                return MapToClientResponse(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el cliente con ID {id}");
                throw;
            }
        }

        public async Task<ClientResponseDTO> CreateClientAsync(CreateClientDTO clientDto, int createdById)
        {
            try
            {
                // Validar que el correo no esté en uso
                if (await _context.Clients.AnyAsync(c => c.email == clientDto.Email && c.Active))
                {
                    throw new InvalidOperationException("El correo electrónico ya está en uso");
                }

                // Validar que el teléfono no esté en uso
                if (await _context.Clients.AnyAsync(c => c.phone == clientDto.Cellphone && c.Active))
                {
                    throw new InvalidOperationException("El número de celular ya está en uso");
                }

                // Validar que la ruta exista
                var routeExists = await _context.Routes.AnyAsync(r => r.Id == clientDto.RouteId);
                if (!routeExists)
                {
                    throw new KeyNotFoundException("La ruta especificada no existe");
                }

                var client = new Client
                {
                    name = clientDto.Name,
                    code = clientDto.Lastname,
                    email = clientDto.Email,
                    phone = clientDto.Cellphone,
                    address = clientDto.Direction,
                    notes = clientDto.Note,
                    RouteId = clientDto.RouteId,
                    creator = createdById,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Active = true
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                // Recargar el cliente con las relaciones
                var createdClient = await _context.Clients
                    .Include(c => c.Route)
                    .FirstOrDefaultAsync(c => c.Id == client.Id);

                return MapToClientResponse(createdClient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el cliente");
                throw;
            }
        }

        public async Task<ClientResponseDTO> UpdateClientAsync(int id, UpdateClientDTO clientDto)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null || !client.Active)
                {
                    throw new KeyNotFoundException("Cliente no encontrado");
                }

                // Validar que el correo no esté en uso por otro cliente
                if (await _context.Clients.AnyAsync(c => c.Id != id && c.email == clientDto.Email && c.Active))
                {
                    throw new InvalidOperationException("El correo electrónico ya está en uso");
                }

                // Validar que el teléfono no esté en uso por otro cliente
                if (await _context.Clients.AnyAsync(c => c.Id != id && c.phone == clientDto.Cellphone && c.Active))
                {
                    throw new InvalidOperationException("El número de celular ya está en uso");
                }

                // Validar que la ruta exista
                var routeExists = await _context.Routes.AnyAsync(r => r.Id == clientDto.RouteId);
                if (!routeExists)
                {
                    throw new KeyNotFoundException("La ruta especificada no existe");
                }

                // Actualizar propiedades
                client.name = clientDto.Name;
                client.code = clientDto.Lastname;
                client.email = clientDto.Email;
                client.phone = clientDto.Cellphone;
                client.address = clientDto.Direction;
                client.notes = clientDto.Note;
                client.RouteId = clientDto.RouteId;
                client.UpdatedAt = DateTime.UtcNow;

                _context.Entry(client).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Recargar el cliente con las relaciones
                var updatedClient = await _context.Clients
                    .Include(c => c.Route)
                    .FirstOrDefaultAsync(c => c.Id == id);

                return MapToClientResponse(updatedClient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el cliente con ID {id}");
                throw;
            }
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null || !client.Active)
                {
                    throw new KeyNotFoundException("Cliente no encontrado");
                }

                // Validar que el cliente no tenga transacciones relacionadas
                //if (await ClientHasTransactionsAsync(id))
                //{
                //    throw new InvalidOperationException("No se puede eliminar el cliente porque tiene transacciones relacionadas");
                //}

                // Eliminación lógica
                client.Active = false;
                client.UpdatedAt = DateTime.UtcNow;
                _context.Entry(client).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el cliente con ID {id}");
                throw;
            }
        }


        private static ClientResponseDTO MapToClientResponse(Client client)
        {
            if (client == null) return null;

            return new ClientResponseDTO
            {
                Id = client.Id,
                Name = client.name ?? string.Empty,
                Lastname = client.code ?? string.Empty,
                RouteId = client.RouteId ?? 0,
                RouteName = client.Route?.route_name ?? string.Empty,
                Direction = client.address ?? string.Empty,
                Cellphone = client.phone ?? string.Empty,
                Email = client.email ?? string.Empty,
                Note = client.notes,
                LandlinePhone = null, // No hay campo en el modelo actual
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt
            };
        }
}
}
