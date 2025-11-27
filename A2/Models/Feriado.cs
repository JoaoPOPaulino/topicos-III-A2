using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Feriado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Nome { get; set; }

        [Required]
        public required DateTime Data { get; set; }

        public string? Uf { get; set; }
    }
}