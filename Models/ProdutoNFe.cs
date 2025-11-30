using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDV.Models
{
    public class ProdutosNFe
    {
        public int Id { get; set; }
        public string CodigoProdNFe { get; set; }
        public string Nome { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public int Quantidade { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantidadeNFe { get; set; }
        public string UnidadeMedidaNFe { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }
        public int CodigoProduto { get; set; }
        public int NotaFiscalId { get; set; }
        public NotaFiscal NotaFiscal { get; set; }
    }
}
