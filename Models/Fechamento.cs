using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDV.Models
{
    public class Fechamento
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime DataFechamento { get; set; }

    }
}
