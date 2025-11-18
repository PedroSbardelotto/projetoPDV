namespace PDV.Models.ViewModels
{
    public class VendaCreateViewModel
    {
        public decimal ValorTotal { get; set; }
        public List<ItensVendaViewModel> Itens { get; set; }
        public int ClienteId { get; set; }
    }
}
