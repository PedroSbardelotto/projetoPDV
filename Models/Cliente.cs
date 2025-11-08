using System.ComponentModel.DataAnnotations;

namespace PDV.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Email { get; set; }
        public string Contato { get; set; }
        [Required]
        [MaxLength(14)]
        public string CPF { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataAtualizacao { get; set; }

    }
}
