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

        [HttpGet("GetExpenseControl")]
        public async Task<IActionResult> GetExpenseControl([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var expenseControl = await _loanAmortizationService.GetExpenseControlAsync(startDate, endDate);

                if (expenseControl == null || !expenseControl.Any())
                {
                    return NotFound("No se encontraron registros de pagos o préstamos en el rango de fechas proporcionado.");
                }

                return Ok(expenseControl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al obtener el control de gastos: {ex.Message}");
            }
        }
    }
}
