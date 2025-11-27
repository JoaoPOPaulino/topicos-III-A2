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

        [HttpGet("TestRate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TestExchangeRate(
            [FromQuery] int moedaBaseId = 1, // BRL
            [FromQuery] int moedaCotadaId = 2, // USD
            [FromQuery] string data = "2023-01-01") // Data Antiga para garantir cache/busca
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
                // Inclui falhas de API ou deserialização
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
                // Usar data atual para evitar muitas chamadas à API
                var hoje = DateTime.Today;

                // Testar com BRL para USD (mais comum)
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