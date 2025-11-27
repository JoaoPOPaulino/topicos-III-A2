// Models/Despesa.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A2.Models
{
    public class Despesa
    {
        [Key]
        public int Id { get; set; }

        public int PrestacaoContasId { get; set; }
        public PrestacaoContas? PrestacaoContas { get; set; }

        public int CategoriaId { get; set; }
        public CategoriaDespesa? Categoria { get; set; }

        public int? FornecedorId { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        public int MoedaId { get; set; }
        public Moeda? Moeda { get; set; }

        [Required]
        public required string Descricao { get; set; }

        public DateTime DataDespesa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TaxaCambio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorEmBrl { get; set; }

        public string? Observacoes { get; set; }

        public ICollection<ComprovanteDespesa>? Comprovantes { get; set; }
    }
}