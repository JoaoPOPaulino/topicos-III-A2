using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Fornecedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Nome { get; set; }

        public string? CnpjCpf { get; set; }

        public string? Endereco { get; set; }

        public string? Telefone { get; set; }

        public string? Email { get; set; }

        public string? Pais { get; set; }

        public bool Ativo { get; set; } = true;
    }
}