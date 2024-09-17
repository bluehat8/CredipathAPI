using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CredipathAPI.Data;
using CredipathAPI.Model;
using CredipathAPI.Services;
using Microsoft.AspNetCore.Routing;

namespace CredipathAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ClientService _clientService;

        public ClientController(ClientService clientService)
        {
            _clientService = clientService;
        }

        // GET: api/Client
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(clients);
        }

        // GET: api/Client/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        // POST: api/Client
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            await _clientService.CreateClientAsync(client);
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        // PUT: api/Client/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {

            var existingClient = await _clientService.GetClientByIdAsync(id);

            if (existingClient == null)
            {
                return NotFound();
            }


            if (client.name != "string")
            {
                existingClient.name = client.name;
            }
            if (client.code != "string")
            {
                existingClient.code = client.code;
            }

            if (client.phone != "string")
            {
                existingClient.phone = client.phone;
            }

            if (client.email != "string")
            {
                existingClient.email = client.email;
            }

            if (client.notes != "string")
            {
                existingClient.notes = client.notes;
            }

            if (client.address != "string")
            {
                existingClient.address = client.address;
            }

            if (client.RouteId != 0)
            {
                existingClient.RouteId = client.RouteId;
            }

            await _clientService.UpdateClientsAsync(existingClient);
            return NoContent();
        }

        // DELETE: api/Client/5 (Eliminación suave)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var deleted = await _clientService.DeleteClientAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
