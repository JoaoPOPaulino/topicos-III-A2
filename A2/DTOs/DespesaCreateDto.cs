using System.ComponentModel.DataAnnotations;

namespace A2.DTOs
{
    public class DespesaCreateDto
    {
        [Required]
        public int CategoriaId { get; set; }

        public int? FornecedorId { get; set; }

        [Required]
        public int MoedaId { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória.")]
        [StringLength(200)]
        public required string Descricao { get; set; }

        [Required]
        public DateTime DataDespesa { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo.")]
        public decimal Valor { get; set; }

        public string? Observacoes { get; set; }

        public List<string>? ArquivosAnexados { get; set; }
    }
}