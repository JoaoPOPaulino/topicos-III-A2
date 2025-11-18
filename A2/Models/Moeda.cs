using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Moeda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public required string Codigo { get; set; }

        public required string Nome { get; set; }
        public string? Simbolo { get; set; }
    }
}
