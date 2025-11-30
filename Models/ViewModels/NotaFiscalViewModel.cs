namespace PDV.Models.ViewModels
{
    public class NotaFiscalViewModel
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string ChaveAcesso { get; set; }
        public DateTime? DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime? DataEntrada { get; set; }
        public string Modelo { get; set; }
        public string Serie { get; set; }
        public string NaturesaOperacao { get; set; }

        // Complementos 
        public List<ProdutoNFe> ListaProdutos { get; set; } = new List<ProdutoNFe>();
        public List<Produto> ListaProdutosCadastrados { get; set; } = new List<Produto>();
        public List<ProdutoFaturamento> ListaProdutosFaturamento { get; set; } = new List<ProdutoFaturamento>();
        public int CodigoFornecedor { get; set; }
        public string NomeFornecedor { get; set; }
        public string CNPJ { get; set; }
        public string InscricaoEstadual { get; set; }
        public bool BotaoGravar { get; set; }

        public bool AbrirModal { get; set; }

        public IFormFile arquivoXML { get; set; }
        public string ConteudoXML { get; set; }
    }

    public class ProdutoNFe
    {
        public string CodigoProdutoNFe { get; set; }
        public string NomeProdutoNFe { get; set; }
        public decimal QuantidadeNFe { get; set; }
        public decimal PrecoProdutoNFe { get; set; }
        public string ValorTotalProdNFe { get; set; }
        public bool Validado { get; set; } = false;
        public Produto Produto { get; set; }
        public decimal QuantidadeInterna { get; set; }
        public string UnidadeMedida { get; set; }
        public bool ProdutoNovo { get; set; }
    }

    public class ProdutoFaturamento
    {
        public string CodigoProduto { get; set; }
        public string NomeProduto { get; set; }
        public string Quantidade { get; set; }
        public string Custo { get; set; }
        public string PrecoVenda { get; set; }
        public string ValorTotal { get; set; }
        public string QuantidadeEstoque { get; set; }
        public string CustoAntigo { get; set; }
        public string NomeCategoria { get; set; }
        public string DataAtualizacao { get; set; }
    }
}
