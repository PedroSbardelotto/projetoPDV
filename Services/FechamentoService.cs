using PDV.Models.Interfaces;
using PDV.Models;
using PDV.Data;

namespace PDV.Services
{
    public class FechamentoService: IFechamentoService
    {
        private readonly PDVContext _context;
        public FechamentoService(PDVContext context)
        {
            _context = context;
        }
        public void AbrirNovoCaixa(decimal valorInicial)
        {
            Fechamento aberturaCaixa = new Fechamento
            {
                ValorAbertura = valorInicial,
                DataAbertura = DateTime.Now
            };
            _context.Fechamento.Add(aberturaCaixa);
            _context.SaveChanges();
        }
        public void FecharCaixa(decimal ValorFechamento)
        {
            var fechamentoCaixa = _context.Fechamento
            .Where(f => f.DataAbertura.Date == DateTime.Today && f.DataFechamento == null)
            .SingleOrDefault();

            if (fechamentoCaixa == null) { return; }

            if (fechamentoCaixa != null)
            {
                fechamentoCaixa.ValorFechamento = ValorFechamento;
                fechamentoCaixa.DataFechamento = DateTime.Now;
                _context.Fechamento.Update(fechamentoCaixa);
                _context.SaveChanges();
            }
        }

    }
}
