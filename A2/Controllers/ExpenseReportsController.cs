using A2.DTOs;
using A2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace A2.Controllers
{
    [ApiController]
    [Route("api/ExpenseReports")]
    public class ExpenseReportsController : ControllerBase
    {
        private readonly IPrestacaoContasService _service;
        private readonly ILogger<ExpenseReportsController> _logger;

        private const int RhAdminId = 1;

        public ExpenseReportsController(
            IPrestacaoContasService service,
            ILogger<ExpenseReportsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromBody] PrestacaoContasCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var prestacao = await _service.CreateAsync(dto, RhAdminId);

                return CreatedAtAction(nameof(Post), new { id = prestacao.Id }, new { id = prestacao.Id, status = prestacao.Status.ToString(), totalDespesas = prestacao.TotalDespesas });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de regra de negócio na criação da prestação.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar prestação de contas.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao processar a requisição.");
            }
        }

    }
}