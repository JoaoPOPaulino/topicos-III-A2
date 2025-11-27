using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class CategoriaDespesa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Nome { get; set; }

        public string? Descricao { get; set; }

        public bool Ativo { get; set; } = true;
    }
}