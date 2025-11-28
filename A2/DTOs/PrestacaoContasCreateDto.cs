using System.ComponentModel.DataAnnotations;

namespace A2.DTOs
{
    public class PrestacaoContasCreateDto
    {
        [Required(ErrorMessage = "O ID do Adiantamento é obrigatório.")]
        public int SolicitacaoAdiantamentoId { get; set; }

        [Required(ErrorMessage = "A lista de despesas não pode ser vazia.")]
        [MinLength(1, ErrorMessage = "Deve haver pelo menos uma despesa.")]
        public List<DespesaCreateDto> Despesas { get; set; } = new List<DespesaCreateDto>();
    }
}