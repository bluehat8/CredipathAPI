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
    public class ExcludedDaysController : ControllerBase
    {
        private readonly ExcludedDaysService excludedDays;

        public ExcludedDaysController(ExcludedDaysService _excludedDays)
        {
            excludedDays = _excludedDays;
        }

        // GET: api/InterestTypes
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<ExcludedDays>>> GetExcludedDays()
        {
            var obj = await excludedDays.GetExcludedDays();
            return Ok(obj);
        }

        // GET: api/InterestTypes/5
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ExcludedDays>> GetExcludedDayid(int id)
        {
            var obj = await excludedDays.GetExcludedDaysbyid(id);

            if (obj == null)
            {
                return NotFound();
            }

            return Ok(obj);
        }

        // POST: api/InterestTypes
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ExcludedDays>> PostExcludedDays(ExcludedDays obj)
        {
            await excludedDays.CreateEcxcludedDayAsync(obj);
            return CreatedAtAction(nameof(GetExcludedDays), new { id = obj.Id }, obj);
        }

        // PUT: api/InterestTypes/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> PutExcludedDaty (int id, ExcludedDays obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            var updated = await excludedDays.UpdateExcludedDayAsync(id, obj);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/InterestTypes/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteExcludedDay(int id)
        {
            var deleted = await excludedDays.SoftDeleteExcludedDayAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
