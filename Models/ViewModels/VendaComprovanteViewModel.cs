using PDV.Models;

namespace PDV.Models.ViewModels
{
    public class VendaComprovanteViewModel
    {
        public Vendas Vendas { get; set; }
        public UsuarioEmpresa UsuarioEmpresa { get; set; }
        public List<ProdutoVendasViewModel> Produtos { get; set; }
        public List<ProdutosVenda> ProdutosVenda { get; set; }
    }
}
