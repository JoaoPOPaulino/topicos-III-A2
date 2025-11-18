using A2.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public int EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }

        public int DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }

        [Required]
        public required string NomeCompleto { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public PerfilUsuario Perfil { get; set; }
    }
}
