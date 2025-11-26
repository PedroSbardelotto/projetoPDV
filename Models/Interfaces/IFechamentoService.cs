namespace PDV.Models.Interfaces
{
    public interface IFechamentoService
    {
        bool AbrirNovoCaixa(decimal valorInicial);
        bool FecharCaixa(decimal ValorFechamento);
    }
}
