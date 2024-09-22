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
    public class PaymentFrequenciesController : ControllerBase
    {
        private readonly PaymentFrequencyService paymentFrequencyService;

        public PaymentFrequenciesController(PaymentFrequencyService _paymentFrequencyService)
        {
            paymentFrequencyService = _paymentFrequencyService;
        }

        // GET: api/InterestTypes
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<Paymentfrequencies>>> GetPay()
        {
            var pay = await paymentFrequencyService.GetAllPaymenteFrequency();
            return Ok(pay);
        }

        // GET: api/InterestTypes/5
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Paymentfrequencies>> GetPaybyid(int id)
        {
            var pay = await paymentFrequencyService.GetPaymentFrequencybyid(id);

            if (pay == null)
            {
                return NotFound();
            }

            return Ok(pay);
        }

        // POST: api/InterestTypes
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Paymentfrequencies>> PostPaymentFrequency(Paymentfrequencies obj)
        {
            await paymentFrequencyService.CreatePaymentFrequencyAsync(obj);
            return CreatedAtAction(nameof(GetPay), new { id = obj.Id }, obj);
        }

        // PUT: api/InterestTypes/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> PutPayFrequency(int id, Paymentfrequencies obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            var updated = await paymentFrequencyService.UpdatePaymentFrequencyFieldsAsync(id, obj);
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
            var deleted = await paymentFrequencyService.SoftDeleteInterestTypeAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
