using A2.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A2.Models
{
    public class PrestacaoContas
    {
        [Key]
        public int Id { get; set; }

        public int SolicitacaoAdiantamentoId { get; set; }
        public SolicitacaoAdiantamento? SolicitacaoAdiantamento { get; set; }

        [ForeignKey("CriadoPor")]
        public int CriadoPorId { get; set; }
        public Usuario? CriadoPor { get; set; }

        public StatusAdiantamento Status { get; set; } = StatusAdiantamento.Revisao;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDespesas { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoReembolso { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoDevolucao { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        public DateTime? EnviadoEm { get; set; }

        public ICollection<Despesa>? Despesas { get; set; }
    }
}