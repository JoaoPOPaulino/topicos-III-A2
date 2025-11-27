using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class LogAuditoria
    {
        [Key]
        public int Id { get; set; }

        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required]
        public required string TipoEntidade { get; set; }

        public int EntidadeId { get; set; }

        [Required]
        public required string Acao { get; set; }

        public string? DadosAnteriores { get; set; }
        public string? DadosPosteriores { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}