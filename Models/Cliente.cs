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
        public string CNPJ { get; set; }
        [Required]
        public string InscricaoEstadual { get; set; }
        public DateTime DataEntrada { get; set; } = DateTime.Now;
        public DateTime DataAtualizacao { get; set; }

    }
}
