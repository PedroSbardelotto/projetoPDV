using System.ComponentModel.DataAnnotations.Schema;

namespace PDV.Models
{
    public class ProdutoFornecedor
    {
        public int Id { get; set; }
        public string CodProdNF { get; set; }
        public int? CodigoNF { get; set; }
        public int CodigoFornecedor { get; set; }
        [ForeignKey("CodigoFornecedor")]
        public Fornecedor? Fornecedor { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
    }
}
