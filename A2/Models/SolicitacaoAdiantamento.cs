using A2.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A2.Models
{
    public class SolicitacaoAdiantamento
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Colaborador")]
        public int ColaboradorId { get; set; }
        public Usuario? Colaborador { get; set; }

        [ForeignKey("CriadoPor")]
        public int CriadoPorId { get; set; }
        public Usuario? CriadoPor { get; set; }

        public int DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }

        public int MoedaId { get; set; }
        public Moeda? Moeda { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [Required]
        public required string Justificativa { get; set; }

        public StatusAdiantamento Status { get; set; } = StatusAdiantamento.Pendente;

        public DateTime DataPagamentoRequerida { get; set; }

        public DateTime? DataPagamentoAjustada { get; set; }

        public string? Observacoes { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        public DateTime? AtualizadoEm { get; set; }
    }
}