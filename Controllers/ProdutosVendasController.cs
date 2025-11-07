using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PDV.Data;
using PDV.Models;

namespace PDV.Controllers
{
    public class ProdutosVendasController : Controller
    {
        private readonly PDVContext _context;

        public ProdutosVendasController(PDVContext context)
        {
            _context = context;
        }

        // GET: ProdutosVendas
        public async Task<IActionResult> Index()
        {
            var pDVContext = _context.ProdutosVenda.Include(p => p.Produto).Include(p => p.Vendas);
            return View(await pDVContext.ToListAsync());
        }

        // GET: ProdutosVendas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProdutosVenda == null)
            {
                return NotFound();
            }

            var produtosVenda = await _context.ProdutosVenda
                .Include(p => p.Produto)
                .Include(p => p.Vendas)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produtosVenda == null)
            {
                return NotFound();
            }

            return View(produtosVenda);
        }

        // GET: ProdutosVendas/Create
        public IActionResult Create()
        {
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome");
            ViewData["VendasId"] = new SelectList(_context.Vendas, "Id", "Id");
            return View();
        }

        // POST: ProdutosVendas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Quantidade,DataEntrada,VendasId,ProdutoId")] ProdutosVenda produtosVenda)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produtosVenda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", produtosVenda.ProdutoId);
            ViewData["VendasId"] = new SelectList(_context.Vendas, "Id", "Id", produtosVenda.VendasId);
            return View(produtosVenda);
        }

        // GET: ProdutosVendas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProdutosVenda == null)
            {
                return NotFound();
            }

            var produtosVenda = await _context.ProdutosVenda.FindAsync(id);
            if (produtosVenda == null)
            {
                return NotFound();
            }
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", produtosVenda.ProdutoId);
            ViewData["VendasId"] = new SelectList(_context.Vendas, "Id", "Id", produtosVenda.VendasId);
            return View(produtosVenda);
        }

        // POST: ProdutosVendas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantidade,DataEntrada,VendasId,ProdutoId")] ProdutosVenda produtosVenda)
        {
            if (id != produtosVenda.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produtosVenda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutosVendaExists(produtosVenda.Id))
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
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", produtosVenda.ProdutoId);
            ViewData["VendasId"] = new SelectList(_context.Vendas, "Id", "Id", produtosVenda.VendasId);
            return View(produtosVenda);
        }

        // GET: ProdutosVendas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProdutosVenda == null)
            {
                return NotFound();
            }

            var produtosVenda = await _context.ProdutosVenda
                .Include(p => p.Produto)
                .Include(p => p.Vendas)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produtosVenda == null)
            {
                return NotFound();
            }

            return View(produtosVenda);
        }

        // POST: ProdutosVendas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProdutosVenda == null)
            {
                return Problem("Entity set 'PDVContext.ProdutosVenda'  is null.");
            }
            var produtosVenda = await _context.ProdutosVenda.FindAsync(id);
            if (produtosVenda != null)
            {
                _context.ProdutosVenda.Remove(produtosVenda);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutosVendaExists(int id)
        {
          return (_context.ProdutosVenda?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
