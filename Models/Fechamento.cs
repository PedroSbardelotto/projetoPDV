using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PDV.Models
{
    public class Fechamento
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Valor Abertura")]
        public decimal ValorAbertura { get; set; }
        [AllowNull]
        [Display(Name = "VAlor Fechamento")]
        public decimal ValorFechamento { get; set; }
        [Display(Name = "Data Abertura")]
        public DateTime DataAbertura { get; set; }
        [Display(Name = "Data Fechamento")]
        public DateTime? DataFechamento { get; set; }

    }
}
