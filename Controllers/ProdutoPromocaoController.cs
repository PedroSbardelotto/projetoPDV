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
    public class ProdutoPromocaoController : Controller
    {
        private readonly PDVContext _context;

        public ProdutoPromocaoController(PDVContext context)
        {
            _context = context;
        }

        // GET: ProdutoPromocao
        public async Task<IActionResult> Index()
        {
              return _context.ProdutoPromocao != null ? 
                          View(await _context.ProdutoPromocao.ToListAsync()) :
                          Problem("Entity set 'PDVContext.ProdutoPromocao'  is null.");
        }

        // GET: ProdutoPromocao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProdutoPromocao == null)
            {
                return NotFound();
            }

            var produtoPromocao = await _context.ProdutoPromocao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produtoPromocao == null)
            {
                return NotFound();
            }

            return View(produtoPromocao);
        }

        // GET: ProdutoPromocao/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProdutoPromocao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Preco,DataInicio,DataFim")] ProdutoPromocao produtoPromocao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produtoPromocao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produtoPromocao);
        }

        // GET: ProdutoPromocao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProdutoPromocao == null)
            {
                return NotFound();
            }

            var produtoPromocao = await _context.ProdutoPromocao.FindAsync(id);
            if (produtoPromocao == null)
            {
                return NotFound();
            }
            return View(produtoPromocao);
        }

        // POST: ProdutoPromocao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Preco,DataInicio,DataFim")] ProdutoPromocao produtoPromocao)
        {
            if (id != produtoPromocao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produtoPromocao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoPromocaoExists(produtoPromocao.Id))
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
            return View(produtoPromocao);
        }

        // GET: ProdutoPromocao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProdutoPromocao == null)
            {
                return NotFound();
            }

            var produtoPromocao = await _context.ProdutoPromocao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produtoPromocao == null)
            {
                return NotFound();
            }

            return View(produtoPromocao);
        }

        // POST: ProdutoPromocao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProdutoPromocao == null)
            {
                return Problem("Entity set 'PDVContext.ProdutoPromocao'  is null.");
            }
            var produtoPromocao = await _context.ProdutoPromocao.FindAsync(id);
            if (produtoPromocao != null)
            {
                _context.ProdutoPromocao.Remove(produtoPromocao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoPromocaoExists(int id)
        {
          return (_context.ProdutoPromocao?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
