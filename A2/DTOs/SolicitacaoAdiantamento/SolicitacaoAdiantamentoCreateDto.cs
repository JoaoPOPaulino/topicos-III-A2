
using System.ComponentModel.DataAnnotations;

namespace A2.DTOs.SolicitacaoAdiantamento
{
    public class SolicitacaoAdiantamentoCreateDto
    {
        [Required(ErrorMessage = "Colaborador é obrigatório")]
        public int ColaboradorId { get; set; }

        [Required(ErrorMessage = "Departamento é obrigatório")]
        public int DepartamentoId { get; set; }

        [Required(ErrorMessage = "Moeda é obrigatória")]
        public int MoedaId { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Justificativa é obrigatória")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Justificativa deve ter entre 10 e 500 caracteres")]
        public string Justificativa { get; set; }

        [Required(ErrorMessage = "Data de pagamento é obrigatória")]
        public DateTime DataPagamentoRequerida { get; set; }

        public string? Observacoes { get; set; }
    }
}