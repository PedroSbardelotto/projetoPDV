using System.ComponentModel.DataAnnotations;

namespace PDV.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Contato { get; set; }
        [Required]
        [MaxLength(14)]
        public string CPF { get; set; }
        public Boolean? Status { get; set; } = true;
        [Display(Name = "Criado em:")]
        public DateTime DataEntrada { get; set; }
        [Display(Name = "Atualizado em:")]
        public DateTime DataAtualizacao { get; set; }

    }
}
