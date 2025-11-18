using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }

        public int EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }

        public required string Nome { get; set; }
        public required string CentroDeCusto { get; set; }

        public int? GestorId { get; set; }
    }
}
