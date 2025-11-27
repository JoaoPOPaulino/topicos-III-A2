using A2.DTOs.SolicitacaoAdiantamento;
using A2.Models.Enums;
using A2.Services;
using Microsoft.AspNetCore.Mvc;

namespace A2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvanceRequestsController : ControllerBase
    {
        private readonly ISolicitacaoAdiantamentoService _service;
        private readonly ILogger<AdvanceRequestsController> _logger; // Para logs

        public AdvanceRequestsController(
            ISolicitacaoAdiantamentoService service,
            ILogger<AdvanceRequestsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            try
            {
                var adiantamentos = await _service.GetAllAsync(search, status, dataInicial, dataFinal);
                return Ok(adiantamentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar adiantamentos.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a requisição.");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var solicitacao = await _service.GetByIdAsync(id);

            if (solicitacao == null)
            {
                return NotFound($"Solicitação ID {id} não encontrada.");
            }

            return Ok(solicitacao);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] SolicitacaoAdiantamentoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int criadoPorId = 1;

            try
            {
                var solicitacao = await _service.CreateAsync(dto, criadoPorId);
                return CreatedAtAction(nameof(GetById), new { id = solicitacao.Id }, solicitacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar adiantamento.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao criar a solicitação.");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] SolicitacaoAdiantamentoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Solicitação ID {id} não encontrada.");
            }
            catch (InvalidOperationException ex)
            {
                
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar adiantamento.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao atualizar a solicitação.");
            }
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] string newStatus)
        {
            if (!Enum.TryParse<StatusAdiantamento>(newStatus, true, out var statusEnum))
            {
                return BadRequest("Status inválido.");
            }

            try
            {
                await _service.ChangeStatusAsync(id, statusEnum);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Solicitação ID {id} não encontrada.");
            }
        }
    }
}