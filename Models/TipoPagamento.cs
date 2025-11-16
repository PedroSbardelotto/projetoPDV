using System.ComponentModel.DataAnnotations;

namespace PDV.Models
{
    public class TipoPagamento
    {
        public int Id { get; set; }
        [Required]
        public string Descricao { get; set; }
        public Boolean Status { get; set; }
    }
}
