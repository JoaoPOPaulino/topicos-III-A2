using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A2.Models
{
    public class AprovacaoAdiantamento
    {
        [Key]
        public int Id { get; set; }

        public int SolicitacaoAdiantamentoId { get; set; }
        public SolicitacaoAdiantamento? SolicitacaoAdiantamento { get; set; }

        [ForeignKey("Aprovador")]
        public int AprovadorId { get; set; }
        public Usuario? Aprovador { get; set; }

        public bool Aprovado { get; set; }

        public string? Comentario { get; set; }

        public DateTime DataAprovacao { get; set; } = DateTime.UtcNow;
    }
}