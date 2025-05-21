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
    public class BankController : Controller
    {
        private readonly BankService _bankService;
        public BankController(BankService bankService)
        {
            _bankService = bankService;
        }

        // GET: api/Client
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bank>>> GetBank()
        {
            var banks = await _bankService.GetAllBankAsync();
            return Ok(banks);
        }

        // GET: api/Client/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bank>> GetBank(int id)
        {
            var bank = await _bankService.GetBankIdAsync(id);

            if (bank == null)
            {
                return NotFound();
            }

            return Ok(bank);
        }

        // POST: api/Client
        [HttpPost]
        public async Task<ActionResult<Bank>> PostBank(Bank bank)
        {
            await _bankService.CreateBankAsync(bank);
            return CreatedAtAction(nameof(GetBank), new { id = bank.Id }, bank);
        }

        // PUT: api/Client/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBank(int id, Bank bank)
        {

            var existingBank = await _bankService.GetBankIdAsync(id);

            if (existingBank == null)
            {
                return NotFound();
            }

            if (existingBank.name != "string" || existingBank.code != "string" || existingBank.email != "string")
            {
                if (existingBank.name != "string") existingBank.name = existingBank.name;
                if (existingBank.code != "string") existingBank.code = existingBank.code;
                if (existingBank.phone != "string") existingBank.phone = existingBank.phone;
                if (existingBank.email != "string") existingBank.email = existingBank.email;
            }

            await _bankService.UpdateBankAsync(existingBank);
            return NoContent();
        }

        // DELETE: api/Client/5 (Eliminación suave)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            var deleted = await _bankService.DeleteBankAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
