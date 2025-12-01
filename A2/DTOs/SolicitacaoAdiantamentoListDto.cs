using A2.Models.Enums;

namespace A2.DTOs.SolicitacaoAdiantamento
{
    public class SolicitacaoAdiantamentoListDto
    {
        public int Id { get; set; }
        public string SolicitanteNome { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string MoedaCodigo { get; set; }
        public string ValorFormatado { get; set; }
        public DateTime DataCriacao { get; set; }
        public StatusAdiantamento Status { get; set; }
        public string StatusDescricao { get; set; }
    }
}