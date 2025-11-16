namespace PDV.Models.Interfaces
{
    public interface IFechamentoService
    {
        void AbrirNovoCaixa(decimal valorInicial);
        void FecharCaixa(decimal ValorFechamento);
    }
}
