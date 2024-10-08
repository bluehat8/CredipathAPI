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
    public class LoansController : ControllerBase
    {

        private readonly LoansService loansService;

        public LoansController(LoansService _loan)
        {
            loansService = _loan;
        }

        // GET: api/InterestTypes
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<Loans>>> Getloan()
        {
            var obj = await loansService.GetAllLoans();
            return Ok(obj);
        }

        // GET: api/InterestTypes/5
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Loans>> Getloanbyid(int id)
        {
            var obj = await loansService.Getloanbyid(id);

            if (obj == null)
            {
                return NotFound();
            }

            return Ok(obj);
        }

        // POST: api/InterestTypes
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Loans>> Postloan(Loans obj)
        {
            await loansService.CreateLoanAsync(obj);
            return CreatedAtAction(nameof(Getloan), new { id = obj.Id }, obj);
        }

        // PUT: api/InterestTypes/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> PutLoan(int id, Loans obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            var updated = await loansService.UpdateLoanAsync(id, obj);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/InterestTypes/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeletePay(int id)
        {
            var deleted = await loansService.SoftDeleteLoanAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
