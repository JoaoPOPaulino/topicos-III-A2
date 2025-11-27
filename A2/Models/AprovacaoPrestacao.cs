using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using A2.Models.Enums;

namespace A2.Models
{
    public class AprovacaoPrestacao
    {
        [Key]
        public int Id { get; set; }

        public int PrestacaoContasId { get; set; }
        public PrestacaoContas? PrestacaoContas { get; set; }

        [ForeignKey("Aprovador")]
        public int AprovadorId { get; set; }
        public Usuario? Aprovador { get; set; }

        public StatusPrestacao Status { get; set; }

        public string? Comentario { get; set; }

        public DateTime DataAprovacao { get; set; } = DateTime.UtcNow;
    }
}