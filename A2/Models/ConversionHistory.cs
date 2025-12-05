using System;

namespace A2.Models
{
    public class ConversionHistory
    {
        public int Id { get; set; }
        public string FromCurrency { get; set; } = null!;
        public string ToCurrency { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal Converted { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; } // armazena data/hora do registro
    }
}
