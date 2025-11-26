namespace PDV.Models.ViewModels
{
    public class PromocaoSalvarViewModel
    {
        // Você usará esta lista para criar os objetos ProdutoPromocao no Controller
        public List<ProdutoPromocaoItem> Produtos { get; set; }
        // Adicione DataInicio e DataFim se forem globais para todos os produtos
        // public DateTime DataInicio { get; set; } 
        // public DateTime DataFim { get; set; }
    }

    public class ProdutoPromocaoItem
    {
        public int ProdutoId { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
