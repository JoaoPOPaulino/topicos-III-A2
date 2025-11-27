using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class ComprovanteDespesa
    {
        [Key]
        public int Id { get; set; }

        public int DespesaId { get; set; }
        public Despesa? Despesa { get; set; }

        [Required]
        public required string NomeArquivo { get; set; }

        [Required]
        public required string CaminhoArquivo { get; set; }

        public string? TipoArquivo { get; set; }

        public long TamanhoArquivo { get; set; }

        public DateTime UploadEm { get; set; } = DateTime.UtcNow;
    }
}