using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A2.Models
{
    // Mapeia para exchange_rate_log
    public class LogCotacao
    {
        [Key]
        public int Id { get; set; }

        public int MoedaBaseId { get; set; }
        public Moeda? MoedaBase { get; set; }

        public int MoedaCotadaId { get; set; }
        public Moeda? MoedaCotada { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal Taxa { get; set; }

        public DateTime DataReferencia { get; set; }
        public DateTime CapturadoEm { get; set; } = DateTime.UtcNow;
    }
}