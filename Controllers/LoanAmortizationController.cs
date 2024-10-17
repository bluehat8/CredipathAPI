using CredipathAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CredipathAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanAmortizationController : ControllerBase
    {
        private readonly LoanAmortizationService _loanAmortizationService;

        public LoanAmortizationController(LoanAmortizationService loanAmortizationService)
        {
            _loanAmortizationService = loanAmortizationService;
        }

        // GET: api/LoanAmortization/overdue
        [HttpGet("overdueAmortization")]
        public async Task<IActionResult> GetOverdueAmortizations()
        {
            await _loanAmortizationService.SetOverdueAmortizationsAsync();

            var overdueAmortizations = await _loanAmortizationService.GetOverdueAmortizationsAsync();
            return Ok(overdueAmortizations);
        }

        // PUT: api/LoanAmortization/set-overdue
        [HttpPut("set-overdue")]
        public async Task<IActionResult> SetOverdueAmortizations()
        {
            var result = await _loanAmortizationService.SetOverdueAmortizationsAsync();

            if (result)
            {
                return Ok(new { message = "Amortizaciones vencidas actualizadas correctamente." });
            }
            return BadRequest(new { message = "No hay amortizaciones vencidas para actualizar." });
        }
    }
}
