
using A2.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A2.Models
{
    public class Pagamento
    {
        [Key]
        public int Id { get; set; }

        public TipoPagamento Tipo { get; set; }

        public int? SolicitacaoAdiantamentoId { get; set; }
        public SolicitacaoAdiantamento? SolicitacaoAdiantamento { get; set; }

        public int? PrestacaoContasId { get; set; }
        public PrestacaoContas? PrestacaoContas { get; set; }

        [ForeignKey("Beneficiario")]
        public int BeneficiarioId { get; set; }
        public Usuario? Beneficiario { get; set; }

        public int MoedaId { get; set; }
        public Moeda? Moeda { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;

        public DateTime DataPagamentoPrevista { get; set; }

        public DateTime? DataPagamentoEfetivada { get; set; }

        [ForeignKey("ProcessadoPor")]
        public int? ProcessadoPorId { get; set; }
        public Usuario? ProcessadoPor { get; set; }

        public string? Observacoes { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}