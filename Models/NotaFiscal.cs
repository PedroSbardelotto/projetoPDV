using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PDV.Models
{
    public class NotaFiscal
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string ChaveAcesso { get; set; }
        public DateTime DataEmissao { get; set; }
        public int CodigoFornecedor { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")] // OK - Configuração explícita
        [Display(Name = "Valor Abertura")]
        public decimal ValorTotal { get; set; }
        public DateTime DataEntrada { get; set; }
        public int Serie { get; set; }
        public string NatuOp { get; set; }
        public List<ProdutosNFe> Produtos { get; set; }

    }
}
