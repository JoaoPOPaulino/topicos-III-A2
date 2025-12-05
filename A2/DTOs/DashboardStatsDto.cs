// A2/DTOs/Dashboard/DashboardStatsDto.cs
namespace A2.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public decimal TotalAdiantamentosPendentes { get; set; }
        public string TotalAdiantamentosPendentesFormatado { get; set; } = string.Empty;
        public int QuantidadeAdiantamentosPendentes { get; set; }

        public decimal DespesasEmRevisao { get; set; }
        public string DespesasEmRevisaoFormatado { get; set; } = string.Empty;
        public int QuantidadeDespesasEmRevisao { get; set; }

        public int PagamentosAtrasados { get; set; }

        public decimal EconomiaMensal { get; set; }
        public string EconomiaMensalFormatado { get; set; } = string.Empty;

        // Trends (variação percentual do mês anterior)
        public decimal TrendAdiantamentos { get; set; }
        public decimal TrendDespesas { get; set; }
        public int TrendPagamentos { get; set; }
        public decimal TrendEconomia { get; set; }
    }

    public class DashboardActivityDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // approval, payment, review
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public int? RelatedEntityId { get; set; }
    }

    public class DashboardChartDto
    {
        public List<ChartDataPoint> AdiantamentosPorStatus { get; set; } = new();
        public List<ChartDataPoint> DespesasPorCategoria { get; set; } = new();
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Value { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}