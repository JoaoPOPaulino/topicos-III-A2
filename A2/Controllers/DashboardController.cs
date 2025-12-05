// A2/Controllers/DashboardController.cs
using A2.Data;
using A2.DTOs.Dashboard;
using A2.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retorna estatísticas gerais do dashboard
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            try
            {
                var hoje = DateTime.Today;
                var inicioMesAtual = new DateTime(hoje.Year, hoje.Month, 1);
                var inicioMesAnterior = inicioMesAtual.AddMonths(-1);

                // 1. Total de Adiantamentos Pendentes (Status 1 ou 2)
                var adiantamentosPendentes = await _context.SolicitacoesAdiantamento
                    .Where(s => s.Status == StatusAdiantamento.Pendente || s.Status == StatusAdiantamento.Revisao)
                    .ToListAsync();

                var totalAdiantamentosPendentes = adiantamentosPendentes.Sum(s => s.Valor);

                var adiantamentosMesAnterior = await _context.SolicitacoesAdiantamento
                    .Where(s => s.CriadoEm >= inicioMesAnterior && s.CriadoEm < inicioMesAtual)
                    .CountAsync();

                var adiantamentosMesAtual = await _context.SolicitacoesAdiantamento
                    .Where(s => s.CriadoEm >= inicioMesAtual)
                    .CountAsync();

                var trendAdiantamentos = adiantamentosMesAnterior > 0
                    ? ((decimal)(adiantamentosMesAtual - adiantamentosMesAnterior) / adiantamentosMesAnterior) * 100
                    : 0;

                // 2. Despesas em Revisão
                var despesasRevisao = await _context.PrestacoesContas
                    .Where(p => p.Status == StatusAdiantamento.Pendente)
                    .Include(p => p.Despesas)
                    .ToListAsync();

                var totalDespesasRevisao = despesasRevisao
                    .SelectMany(p => p.Despesas ?? new List<Models.Despesa>())
                    .Sum(d => d.Valor);

                // 3. Pagamentos Atrasados
                var pagamentosAtrasados = await _context.SolicitacoesAdiantamento
                    .Where(s => s.DataPagamentoAjustada < hoje
                             && s.Status != StatusAdiantamento.Pago
                             && s.Status != StatusAdiantamento.Finalizado
                             && s.Status != StatusAdiantamento.Rejeitado)
                    .CountAsync();

                // 4. Economia Mensal
                var economiaMensal = 1280.00m;

                var stats = new DashboardStatsDto
                {
                    TotalAdiantamentosPendentes = totalAdiantamentosPendentes,
                    TotalAdiantamentosPendentesFormatado = $"R$ {totalAdiantamentosPendentes:N2}",
                    QuantidadeAdiantamentosPendentes = adiantamentosPendentes.Count,

                    DespesasEmRevisao = totalDespesasRevisao,
                    DespesasEmRevisaoFormatado = $"R$ {totalDespesasRevisao:N2}",
                    QuantidadeDespesasEmRevisao = despesasRevisao.Count,

                    PagamentosAtrasados = pagamentosAtrasados,

                    EconomiaMensal = economiaMensal,
                    EconomiaMensalFormatado = $"R$ {economiaMensal:N2}",

                    TrendAdiantamentos = Math.Round(trendAdiantamentos, 1),
                    TrendDespesas = -5.0m,
                    TrendPagamentos = -2,
                    TrendEconomia = 8.0m
                };

                _logger.LogInformation("Estatísticas calculadas com sucesso");
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas do dashboard");
                return StatusCode(500, new { error = "Erro ao processar estatísticas", details = ex.Message });
            }
        }

        /// <summary>
        /// Retorna atividades recentes (últimas 10)
        /// </summary>
        [HttpGet("activities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DashboardActivityDto>>> GetRecentActivities()
        {
            try
            {
                var activities = new List<DashboardActivityDto>();

                // Busca logs de auditoria recentes
                var logs = await _context.LogsAuditoria
                    .OrderByDescending(l => l.CriadoEm)
                    .Take(10)
                    .Include(l => l.Usuario)
                    .ToListAsync();

                foreach (var log in logs)
                {
                    var activity = new DashboardActivityDto
                    {
                        Id = log.Id,
                        UserName = log.Usuario?.NomeCompleto ?? "Sistema",
                        CreatedAt = log.CriadoEm,
                        TimeAgo = GetTimeAgo(log.CriadoEm),
                        RelatedEntityId = log.EntidadeId
                    };

                    // Determina tipo e ação
                    switch (log.Acao.ToUpper())
                    {
                        case "CREATE":
                            activity.Type = "approval";
                            activity.Action = log.TipoEntidade == "SolicitacaoAdiantamento"
                                ? "criou um adiantamento"
                                : "criou uma prestação";
                            break;

                        case "UPDATE":
                            activity.Type = "review";
                            activity.Action = "atualizou um registro";
                            break;

                        case var a when a.Contains("APROVADO"):
                            activity.Type = "approval";
                            activity.Action = "aprovou um adiantamento";
                            break;

                        case var a when a.Contains("PAGO"):
                            activity.Type = "payment";
                            activity.Action = "realizou um pagamento";
                            break;

                        default:
                            activity.Type = "review";
                            activity.Action = log.Acao.ToLower();
                            break;
                    }

                    activities.Add(activity);
                }

                _logger.LogInformation("Atividades carregadas: {Count}", activities.Count);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar atividades recentes");
                return Ok(new List<DashboardActivityDto>()); // Retorna lista vazia em vez de erro
            }
        }

        /// <summary>
        /// Retorna dados para gráficos
        /// </summary>
        [HttpGet("charts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DashboardChartDto>> GetChartData()
        {
            try
            {
                var chartData = new DashboardChartDto
                {
                    AdiantamentosPorStatus = new List<ChartDataPoint>(),
                    DespesasPorCategoria = new List<ChartDataPoint>()
                };

                // 1. Adiantamentos por Status
                try
                {
                    var adiantamentosPorStatus = await _context.SolicitacoesAdiantamento
                        .GroupBy(s => s.Status)
                        .Select(g => new
                        {
                            Status = g.Key,
                            Count = g.Count(),
                            Value = g.Sum(s => s.Valor)
                        })
                        .ToListAsync();

                    chartData.AdiantamentosPorStatus = adiantamentosPorStatus.Select(g => new ChartDataPoint
                    {
                        Label = g.Status.ToString(),
                        Count = g.Count,
                        Value = g.Value,
                        Color = GetStatusColor(g.Status)
                    }).ToList();

                    _logger.LogInformation("Adiantamentos por status: {Count}", chartData.AdiantamentosPorStatus.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao buscar adiantamentos por status");
                }

                // 2. Despesas por Categoria
                try
                {
                    var despesasExistem = await _context.Despesas.AnyAsync();

                    if (despesasExistem)
                    {
                        // ✅ CORREÇÃO: Removido ?? 0 e != null
                        var despesasPorCategoria = await _context.Despesas
                            .Where(d => d.CategoriaId > 0)
                            .GroupBy(d => d.CategoriaId)
                            .Select(g => new
                            {
                                CategoriaId = g.Key,
                                Count = g.Count(),
                                Value = g.Sum(d => d.Valor)
                            })
                            .ToListAsync();

                        if (despesasPorCategoria.Any())
                        {
                            var categoriaIds = despesasPorCategoria.Select(d => d.CategoriaId).ToList();
                            var categorias = await _context.CategoriasDespesa
                                .Where(c => categoriaIds.Contains(c.Id))
                                .ToListAsync();

                            chartData.DespesasPorCategoria = despesasPorCategoria.Select(d =>
                            {
                                var categoria = categorias.FirstOrDefault(c => c.Id == d.CategoriaId);
                                return new ChartDataPoint
                                {
                                    Label = categoria?.Nome ?? $"Categoria {d.CategoriaId}",
                                    Count = d.Count,
                                    Value = d.Value,
                                    Color = GetCategoryColor(d.CategoriaId)
                                };
                            }).ToList();
                        }
                    }

                    _logger.LogInformation("Despesas por categoria: {Count}", chartData.DespesasPorCategoria.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao buscar despesas por categoria");
                }

                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro geral ao buscar dados dos gráficos");

                // Retorna dados vazios em vez de erro
                return Ok(new DashboardChartDto
                {
                    AdiantamentosPorStatus = new List<ChartDataPoint>(),
                    DespesasPorCategoria = new List<ChartDataPoint>()
                });
            }
        }

        // Métodos auxiliares
        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "agora mesmo";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} min atrás";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hora{((int)timeSpan.TotalHours > 1 ? "s" : "")} atrás";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} dia{((int)timeSpan.TotalDays > 1 ? "s" : "")} atrás";

            return dateTime.ToString("dd/MM/yyyy");
        }

        private string GetStatusColor(StatusAdiantamento status)
        {
            return status switch
            {
                StatusAdiantamento.Pendente => "#F2C94C",
                StatusAdiantamento.Revisao => "#004AAD",
                StatusAdiantamento.Aprovado => "#00B37E",
                StatusAdiantamento.Atrasado => "#F59734",
                StatusAdiantamento.Rejeitado => "#E63946",
                StatusAdiantamento.Pago => "#00B37E",
                StatusAdiantamento.PrestacaoEnviada => "#006064",
                StatusAdiantamento.Finalizado => "#2E7D32",
                _ => "#9CA3AF"
            };
        }

        private string GetCategoryColor(int categoryId)
        {
            var colors = new[] { "#004AAD", "#00B37E", "#E63946", "#F59734", "#9A7B00", "#6B7280" };
            return colors[categoryId % colors.Length];
        }
    }
}