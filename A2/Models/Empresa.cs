using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da empresa é obrigatório")]
        public required string Nome { get; set; }

        [Required]
        public required string Cnpj { get; set; }

        public ICollection<Departamento>? Departamentos { get; set; }
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
