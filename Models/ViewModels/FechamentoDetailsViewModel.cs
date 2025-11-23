namespace PDV.Models.ViewModels
{
    public class FechamentoDetailsViewModel
    {
        public Fechamento Fechamento { get; set; }
        public List<Vendas> Vendas { get; set; }
        public decimal? VendasTotal { get; set; }
    }
}
