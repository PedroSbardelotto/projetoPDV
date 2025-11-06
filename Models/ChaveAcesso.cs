using System.ComponentModel.DataAnnotations;

namespace PDV.Models
{
    public class ChaveAcesso
    {
        public int Id { get; set; }
        [Required]
        public string UUID { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
