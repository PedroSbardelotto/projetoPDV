using PDV.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        public bool AbrirNovoCaixa(decimal valorInicial)
        {
            var caixaAberto = _context.Fechamento.Where(f => f.DataFechamento == null).SingleOrDefault();
            if(caixaAberto != null)
            {
                return false;
            }
            Fechamento aberturaCaixa = new Fechamento
            {
                ValorAbertura = valorInicial,
                DataAbertura = DateTime.Now
            };
            _context.Fechamento.Add(aberturaCaixa);
            _context.SaveChanges();
            return true;
        }
        public bool FecharCaixa(decimal ValorFechamento)
        {
            var fechamentoCaixa = _context.Fechamento
            .Where(f => f.DataFechamento == null)
            .SingleOrDefault();

            if (fechamentoCaixa == null) { return false; }

            fechamentoCaixa.ValorFechamento = ValorFechamento;
            fechamentoCaixa.DataFechamento = DateTime.Now;
            _context.Fechamento.Update(fechamentoCaixa);
            _context.SaveChanges();
            return true;            
        }

    }
}
