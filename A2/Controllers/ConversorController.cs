using A2.Data;
using A2.DTOs;
using A2.Models;
using A2.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<ConversorController> _logger;

        public ConversorController(ApplicationDbContext context, ICurrencyService currencyService, ILogger<ConversorController> logger)
        {
            _context = context;
            _currencyService = currencyService;
            _logger = logger;
        }

        // GET: api/conversor/convert?from=BRL&to=USD&amount=100
        [HttpGet("convert")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Convert([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || amount <= 0)
            {
                return BadRequest("Parâmetros inválidos.");
            }

            try
            {
                var result = await _currencyService.GetConversionAsync(from, to, amount);

                // Salva no histórico (global — sem usuário)
                var history = new ConversionHistory
                {
                    FromCurrency = result.From,
                    ToCurrency = result.To,
                    Amount = result.Amount,
                    Converted = result.Converted,
                    Rate = result.Rate,
                    Date = DateTime.Now
                };

                _context.ConversionHistories.Add(history);
                await _context.SaveChangesAsync();

                return Ok(result);
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Erro ao consultar API externa de câmbio.");
                return StatusCode(StatusCodes.Status502BadGateway, "Erro ao consultar o serviço de câmbio externo.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao processar conversão.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno de servidor.");
            }
        }

        // GET: api/conversor/history
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<ConversionHistoryDto>>> GetHistory()
        {
            var list = await _context.ConversionHistories
                .OrderByDescending(h => h.Id)
                .Take(100) // limite para evitar trazer tudo
                .ToListAsync();

            var dto = list.Select(h => new ConversionHistoryDto
            {
                Id = h.Id,
                From = h.FromCurrency,
                To = h.ToCurrency,
                Amount = h.Amount,
                Converted = h.Converted,
                Rate = h.Rate,
                Date = h.Date.ToString("g", new System.Globalization.CultureInfo("pt-BR"))
            }).ToList();

            return Ok(dto);
        }

        // Opcional: endpoint para limpar histórico (se quiser)
        [HttpDelete("history/clear")]
        public async Task<IActionResult> ClearHistory()
        {
            _context.ConversionHistories.RemoveRange(_context.ConversionHistories);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
