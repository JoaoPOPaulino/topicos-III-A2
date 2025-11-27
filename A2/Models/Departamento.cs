// Models/Departamento.cs
using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Nome { get; set; }

        [Required]
        public required string CentroDeCusto { get; set; }

        public int? GestorId { get; set; }

        public bool Ativo { get; set; } = true;

        public ICollection<Usuario>? Usuarios { get; set; }
    }
}