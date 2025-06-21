using CredipathAPI.DTOs;
using CredipathAPI.Model;

namespace CredipathAPI.Services
{
    public interface IClientService
    {
        Task<PaginatedResponse<ClientResponseDTO>> GetClientsAsync(ClientQueryParams queryParams);
        Task<ClientResponseDTO> GetClientByIdAsync(int id);
        Task<ClientResponseDTO> CreateClientAsync(CreateClientDTO clientDto, int createdById);
        Task<ClientResponseDTO> UpdateClientAsync(int id, UpdateClientDTO clientDto);
        Task<bool> DeleteClientAsync(int id);
    }
}
