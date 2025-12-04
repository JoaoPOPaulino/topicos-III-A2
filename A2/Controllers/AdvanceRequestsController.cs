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
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<AdvanceRequestsController> _logger;

        public AdvanceRequestsController(
            ISolicitacaoAdiantamentoService service,
            IExchangeRateService exchangeRateService,
            ILogger<AdvanceRequestsController> logger)
        {
            _service = service;
            _exchangeRateService = exchangeRateService;
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
            _logger.LogInformation("GET /AdvanceRequests/{Id} recebido.", id);

            try
            {
                var solicitacao = await _service.GetByIdAsync(id);

                if (solicitacao == null)
                {
                    _logger.LogWarning("Solicitação ID {Id} não encontrada.", id);
                    return NotFound($"Solicitação ID {id} não encontrada.");
                }

                _logger.LogDebug("Detalhes do Adiantamento ID {Id} retornados com sucesso.", id);
                return Ok(solicitacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro fatal ao buscar detalhes do Adiantamento ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao buscar detalhes.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] SolicitacaoAdiantamentoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                // ✅ CORRIGIDO: Removi o "id" que não existe aqui
                _logger.LogWarning("Modelo de DTO inválido para POST. Erros: {Errors}",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
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
                _logger.LogWarning("Modelo de DTO inválido para PUT ID {Id}. Erros: {Errors}",
                    id, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
                return BadRequest(ModelState);
            }

            try
            {
                await _service.UpdateAsync(id, dto);
                _logger.LogInformation("✅ Adiantamento ID {Id} atualizado com sucesso.", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Solicitação ID {Id} não encontrada para atualização.", id);
                return NotFound($"Solicitação ID {id} não encontrada.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Operação inválida ao atualizar ID {Id}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar adiantamento ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao atualizar a solicitação.");
            }
        }

        // ✅ CORRIGIDO: Aceita int no query parameter
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] int newStatus)
        {
            _logger.LogInformation("PATCH /AdvanceRequests/{Id}/status recebido. Novo status: {Status}", id, newStatus);

            // ✅ Valida se o int recebido é um valor válido do enum
            if (!Enum.IsDefined(typeof(StatusAdiantamento), newStatus))
            {
                _logger.LogWarning("Status {Status} inválido para ID {Id}.", newStatus, id);
                return BadRequest($"Status {newStatus} inválido. Valores aceitos: 1-8.");
            }

            var statusEnum = (StatusAdiantamento)newStatus;

            try
            {
                await _service.ChangeStatusAsync(id, statusEnum);
                _logger.LogInformation("✅ Status do Adiantamento ID {Id} alterado para {Status}.", id, statusEnum);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Solicitação ID {Id} não encontrada para mudança de status.", id);
                return NotFound($"Solicitação ID {id} não encontrada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do adiantamento ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao alterar status.");
            }
        }

        [HttpGet("TestRate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TestExchangeRate(
            [FromQuery] int moedaBaseId = 1,
            [FromQuery] int moedaCotadaId = 2,
            [FromQuery] string data = "2023-01-01")
        {
            if (!DateTime.TryParse(data, out var dataCambiaria))
            {
                return BadRequest("Formato de data inválido.");
            }

            try
            {
                var rate = await _exchangeRateService.GetRateAsync(moedaBaseId, moedaCotadaId, dataCambiaria);
                return Ok(new
                {
                    MoedaBaseId = moedaBaseId,
                    MoedaCotadaId = moedaCotadaId,
                    DataReferencia = dataCambiaria.ToShortDateString(),
                    Taxa = rate,
                    Mensagem = "Cotação obtida com sucesso (pode ser do cache ou da API externa)."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro na API de Câmbio.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("TestRateSafe")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TestExchangeRateSafe()
        {
            try
            {
                var hoje = DateTime.Today;
                var rate = await _exchangeRateService.GetRateAsync(1, 2, hoje);
                return Ok(new
                {
                    MoedaBase = "BRL",
                    MoedaCotada = "USD",
                    DataReferencia = hoje.ToShortDateString(),
                    Taxa = rate,
                    Observacao = "Taxa obtida (cache/API/fallback)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter taxa de câmbio");
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }
}