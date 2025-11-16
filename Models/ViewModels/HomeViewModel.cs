namespace PDV.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Categoria> Categorias { get; set; }
        public List<Produto> Produtos { get; set; }
        public List<Cliente> Clientes { get; set; }
        public List<TipoPagamento> TipoPagamento { get; set; }
        public Fechamento Fechamento { get; set; }
    }
}
