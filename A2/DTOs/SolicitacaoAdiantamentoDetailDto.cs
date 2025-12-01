using A2.DTOs.SolicitacaoAdiantamento;

namespace A2.DTOs
{
    public class SolicitacaoAdiantamentoDetailDto : SolicitacaoAdiantamentoListDto
    {
        public string JustificativaCompleta { get; set; } = string.Empty;
        public string DepartamentoNome { get; set; } = string.Empty;
        public DateTime DataPagamentoRequerida { get; set; }
        public DateTime? DataPagamentoAjustada { get; set; }
        public string? Observacoes { get; set; }
        public string CriadoPorNome { get; set; } = string.Empty;

    
        public List<string> Anexos { get; set; } = new List<string> { "Recibo1.pdf", "NotaHotel.png" };
    }
}
