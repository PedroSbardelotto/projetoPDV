using System.ComponentModel.DataAnnotations;

namespace PDV.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }
        [Required]
        public string NomeRazao { get; set; }
        [Required]
        public string NomeFantasia { get; set; }
        [Required]
        [MaxLength(18)]
        public string CNPJ { get; set; }
        [Required]
        public string InscricaoEstadual { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Endereco { get; set; }
        [Required]
        public string Contato { get; set; }
        public Boolean? Status { get; set; } = true;
        public DateTime DataEntrada { get; set; } = DateTime.Now;
        public DateTime DataAtualizacao { get; set; }

    }
}
