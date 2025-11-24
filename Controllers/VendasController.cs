using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PDV.Data;
using PDV.Enums;
using PDV.Models;

namespace PDV.Controllers
{
    public class VendasController : Controller
    {
        private readonly PDVContext _context;

        public VendasController(PDVContext context)
        {
            _context = context;
        }

        // GET: Vendas
        public async Task<IActionResult> Index()
        {
            var pDVContext = _context.Vendas.Include(v => v.Cliente).Include(v => v.Fechamento).Include(v => v.TipoPagamento).Include(v => v.UsuarioEmpresa);
            return View(await pDVContext.ToListAsync());
        }

        // GET: Vendas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vendas == null)
            {
                return NotFound();
            }

            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Fechamento)
                .Include(v => v.TipoPagamento)
                .Include(v => v.UsuarioEmpresa)
                .Include(v => v.ProdutosVenda)
                    .ThenInclude(pv => pv.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendas == null)
            {
                return NotFound();
            }

            var somaValores = vendas.ProdutosVenda.Sum(x => x.Produto.Preco * x.Quantidade);

            if (vendas.ValorTotal != somaValores)
            {
                var novoValor = somaValores - vendas.ValorTotal;
                vendas.ValorTotal = somaValores - novoValor;
            }

            return View(vendas);
        }

        // GET: Vendas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "CNPJ");
            ViewData["FechamentoId"] = new SelectList(_context.Fechamento, "Id", "Id");
            ViewData["TipoPagamentoId"] = new SelectList(_context.TipoPagamento, "Id", "Descricao");
            ViewData["UsuarioEmpresaId"] = new SelectList(_context.UsuarioEmpresa, "Id", "CNPJ");
            return View();
        }

        // POST: Vendas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ValorTotal,TipoPagamentoId,ClienteId,FechamentoId,ProdutosVenda")] Vendas vendas)
        {
                DateTime dtAtual = DateTime.Now;

                Fechamento objFechamento = await _context.Fechamento.Where(f => f.DataFechamento == null).FirstOrDefaultAsync();

                if (vendas.ClienteId != null)
                {
                    vendas.Status = (short)Situacao.Pendente;
                    vendas.TipoPagamentoId = null;
                }
                else
                    vendas.Status = (short)Situacao.Finalizado;

                vendas.FechamentoId = objFechamento.Id;
                vendas.DataEntrada = dtAtual;
                vendas.DataAtualizacao = dtAtual;
                vendas.ValorTotal = Convert.ToDecimal(vendas.ValorTotal);

                _context.Add(vendas);
                await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: Vendas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vendas == null)
            {
                return NotFound();
            }

            var vendas = await _context.Vendas.FindAsync(id);
            if (vendas == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "CNPJ", vendas.ClienteId);
            ViewData["FechamentoId"] = new SelectList(_context.Fechamento, "Id", "Id", vendas.FechamentoId);
            ViewData["TipoPagamentoId"] = new SelectList(_context.TipoPagamento, "Id", "Descricao", vendas.TipoPagamentoId);
            ViewData["UsuarioEmpresaId"] = new SelectList(_context.UsuarioEmpresa, "Id", "CNPJ", vendas.UsuarioEmpresaId);
            return View(vendas);
        }

        // POST: Vendas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ValorTotal,Status,DataEntrada,DataAtualizacao,TipoPagamentoId,ClienteId,UsuarioEmpresaId,FechamentoId")] Vendas vendas)
        {
            if (id != vendas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendasExists(vendas.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "CNPJ", vendas.ClienteId);
            ViewData["FechamentoId"] = new SelectList(_context.Fechamento, "Id", "Id", vendas.FechamentoId);
            ViewData["TipoPagamentoId"] = new SelectList(_context.TipoPagamento, "Id", "Descricao", vendas.TipoPagamentoId);
            ViewData["UsuarioEmpresaId"] = new SelectList(_context.UsuarioEmpresa, "Id", "CNPJ", vendas.UsuarioEmpresaId);
            return View(vendas);
        }

        // GET: Vendas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vendas == null)
            {
                return NotFound();
            }

            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Fechamento)
                .Include(v => v.TipoPagamento)
                .Include(v => v.UsuarioEmpresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendas == null)
            {
                return NotFound();
            }

            return View(vendas);
        }

        // POST: Vendas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vendas == null)
            {
                return Problem("Entity set 'PDVContext.Vendas'  is null.");
            }
            var vendas = await _context.Vendas.FindAsync(id);
            if (vendas != null)
            {
                _context.Vendas.Remove(vendas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendasExists(int id)
        {
            return (_context.Vendas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
