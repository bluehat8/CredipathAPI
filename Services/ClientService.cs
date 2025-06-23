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
                        (c.Name != null && c.Name.ToLower().Contains(searchTerm)) ||
                        (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                        (c.Phone != null && c.Phone.Contains(searchTerm)));
                }

                if (queryParams.RouteId.HasValue)
                {
                    query = query.Where(c => c.RouteId == queryParams.RouteId.Value);
                }

                // Contar total antes de paginar
                var totalItems = await query.CountAsync();

                // Aplicar paginación
                var clients = await query
                    .OrderBy(c => c.Name)
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
                // Validar que la identificación no esté en uso
                if (await _context.Clients.AnyAsync(c => c.Identification == clientDto.Identification && c.Active))
                {
                    throw new InvalidOperationException("La identificación ya está en uso");
                }

                // Validar que el correo no esté en uso si se proporciona
                if (!string.IsNullOrEmpty(clientDto.Email) && 
                    await _context.Clients.AnyAsync(c => c.Email == clientDto.Email && c.Active))
                {
                    throw new InvalidOperationException("El correo electrónico ya está en uso");
                }

                // Validar que el teléfono no esté en uso
                if (await _context.Clients.AnyAsync(c => c.Phone == clientDto.Phone && c.Active))
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
                    Identification = clientDto.Identification,
                    Name = clientDto.Name,
                    Code = clientDto.Code,
                    Email = clientDto.Email,
                    Phone = clientDto.Phone,
                    LandlinePhone = clientDto.LandlinePhone,
                    HomeAddress = clientDto.HomeAddress,
                    BusinessAddress = clientDto.BusinessAddress,
                    Gender = clientDto.Gender,
                    Municipality = clientDto.Municipality,
                    City = clientDto.City,
                    Neighborhood = clientDto.Neighborhood,
                    Notes = clientDto.Notes,
                    RouteId = clientDto.RouteId,
                    CreatorId = createdById,
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

                // Validar que la identificación no esté en uso por otro cliente
                if (await _context.Clients.AnyAsync(c => c.Id != id && c.Identification == clientDto.Identification && c.Active))
                {
                    throw new InvalidOperationException("La identificación ya está en uso");
                }

                // Validar que el correo no esté en uso por otro cliente (si se proporciona)
                if (!string.IsNullOrEmpty(clientDto.Email) && 
                    await _context.Clients.AnyAsync(c => c.Id != id && c.Email == clientDto.Email && c.Active))
                {
                    throw new InvalidOperationException("El correo electrónico ya está en uso");
                }

                // Validar que la ruta exista
                var routeExists = await _context.Routes.AnyAsync(r => r.Id == clientDto.RouteId);
                if (!routeExists)
                {
                    throw new KeyNotFoundException("La ruta especificada no existe");
                }

                // Actualizar propiedades
                client.Identification = clientDto.Identification;
                client.Name = clientDto.Name;
                client.Code = clientDto.Code;
                client.Email = clientDto.Email;
                client.Phone = clientDto.Phone;
                client.LandlinePhone = clientDto.LandlinePhone;
                client.HomeAddress = clientDto.HomeAddress;
                client.BusinessAddress = clientDto.BusinessAddress;
                client.Gender = clientDto.Gender;
                client.Municipality = clientDto.Municipality;
                client.City = clientDto.City;
                client.Neighborhood = clientDto.Neighborhood;
                client.Notes = clientDto.Notes;
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
                Identification = client.Identification,
                Name = client.Name,
                Code = client.Code,
                RouteId = client.RouteId ?? 0,
                RouteName = client.Route?.route_name ?? string.Empty,
                HomeAddress = client.HomeAddress,
                BusinessAddress = client.BusinessAddress,
                Gender = client.Gender,
                Municipality = client.Municipality,
                City = client.City,
                Neighborhood = client.Neighborhood,
                Email = client.Email,
                Phone = client.Phone,
                LandlinePhone = client.LandlinePhone,
                Notes = client.Notes,
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt
            };
        }
}
}
