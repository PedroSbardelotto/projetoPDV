using System.ComponentModel.DataAnnotations.Schema;

namespace PDV.Models
{
    public class ProdutosVenda
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataEntrada { get; set; } = DateTime.Now;

        public int VendasId { get; set; }
        public Vendas Vendas { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        
    }
}
