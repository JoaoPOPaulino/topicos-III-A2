using System.ComponentModel.DataAnnotations;
using A2.Models.Enums;

namespace A2.DTOs.SolicitacaoAdiantamento
{
    public class SolicitacaoAdiantamentoCreateDto
    {
        [Required]
        public int ColaboradorId { get; set; }

        [Required]
        [StringLength(255)]
        public required string Justificativa { get; set; }

        [Required]
        public int DepartamentoId { get; set; }

        [Required]
        public int MoedaId { get; set; } 

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Valor { get; set; }

        [Required]
        public DateTime DataPagamentoRequerida { get; set; }

    }
}