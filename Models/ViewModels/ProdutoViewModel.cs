namespace PDV.Models.ViewModels
{
    public class ProdutoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string NomeCategoria { get; set; }

        // O preço base original do produto
        public decimal PrecoBase { get; set; }

        // Se a promoção estiver ativa, este será o preço promocional
        public decimal PrecoPromocional { get; set; }

        // Indica o preço final que deve ser exibido na tabela
        public decimal PrecoExibicao => PrecoPromocional > 0 ? PrecoPromocional : PrecoBase;

        // Se a promoção for aplicada, terá uma data fim para o ícone de expiração
        public DateTime? DataFimPromocao { get; set; }
    }
}
