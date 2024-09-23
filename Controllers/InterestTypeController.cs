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
using Microsoft.AspNetCore.Authorization;

namespace CredipathAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterestTypeController : ControllerBase
    {
        private readonly InterestTypeService _interestTypesService;

        public InterestTypeController(InterestTypeService interestTypesService)
        {
            _interestTypesService = interestTypesService;
        }

        // GET: api/InterestTypes
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<InterestTypes>>> GetInterestTypes()
        {
            var interestTypes = await _interestTypesService.GetAllInterestTypesAsync();
            return Ok(interestTypes);
        }

        // GET: api/InterestTypes/5
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<InterestTypes>> GetInterestType(int id)
        {
            var interestType = await _interestTypesService.GetInterestTypeByIdAsync(id);

            if (interestType == null)
            {
                return NotFound();
            }

            return Ok(interestType);
        }

        // POST: api/InterestTypes
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<InterestTypes>> PostInterestType(InterestTypes interestType)
        {
            await _interestTypesService.CreateInterestTypeAsync(interestType);
            return CreatedAtAction(nameof(GetInterestType), new { id = interestType.Id }, interestType);
        }

        // PUT: api/InterestTypes/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> PutInterestType(int id, InterestTypes interestType)
        {
            if (id != interestType.Id)
            {
                return BadRequest();
            }

            var updated = await _interestTypesService.UpdateInterestTypeFieldsAsync(id, interestType);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/InterestTypes/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteInterestType(int id)
        {
            var deleted = await _interestTypesService.SoftDeleteInterestTypeAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
