using System.ComponentModel.DataAnnotations;

namespace PDV.Models
{
    public class UsuarioEmpresa
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
        public string? Senha { get; set; }
        [Required]
        public string Contato { get; set; }
        public DateTime DataEntrada { get; set; } = DateTime.Now;

        public int? ChaveAcessoId { get; set; }
        public ChaveAcesso? ChaveAcesso { get; set; }
    }
}
