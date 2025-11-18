using A2.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A2.Models
{
    public class SolicitacaoAdiantamento
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Requerente")]
        public int RequerenteId { get; set; }
        public Usuario? Requerente { get; set; }
        public int DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }

        public int MoedaId { get; set; }
        public Moeda? Moeda { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        public StatusAdiantamento Status { get; set; } = StatusAdiantamento.Pendente;

        public DateTime DataPagamentoRequerida { get; set; }
        public DateTime? DataPagamentoAjustada { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
