using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PDV.Data;
using PDV.Enums;
using PDV.Models;
using PDV.Models.ViewModels;

namespace PDV.Controllers
{
    public class FechamentoesController : Controller
    {
        private readonly PDVContext _context;

        public FechamentoesController(PDVContext context)
        {
            _context = context;
        }

        // GET: Fechamentoes
        public async Task<IActionResult> Index()
        {
              return _context.Fechamento != null ? 
                          View(await _context.Fechamento.ToListAsync()) :
                          Problem("Entity set 'PDVContext.Fechamento'  is null.");
        }

        // GET: Fechamentoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fechamento == null)
            {
                return NotFound();
            }
            FechamentoDetailsViewModel fechamento = new FechamentoDetailsViewModel();
            fechamento.Fechamento = await _context.Fechamento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fechamento == null)
            {
                return NotFound();
            }
            fechamento.Vendas = await _context.Vendas.Include(v=> v.TipoPagamento).Include(v=> v.Cliente)
                .Where(v => v.FechamentoId == id).ToListAsync();
            fechamento.VendasTotal = fechamento.Vendas.Where(v => v.Status == (short)Situacao.Finalizado).Sum(v=> v.ValorTotal);
            return View(fechamento);
        }

        // GET: Fechamentoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fechamentoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Valor,DataAbertura,DataFechamento")] Fechamento fechamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fechamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fechamento);
        }

        // GET: Fechamentoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fechamento == null)
            {
                return NotFound();
            }

            var fechamento = await _context.Fechamento.FindAsync(id);
            if (fechamento == null)
            {
                return NotFound();
            }
            return View(fechamento);
        }

        // POST: Fechamentoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Valor,DataAbertura,DataFechamento")] Fechamento fechamento)
        {
            if (id != fechamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fechamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FechamentoExists(fechamento.Id))
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
            return View(fechamento);
        }

        // GET: Fechamentoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fechamento == null)
            {
                return NotFound();
            }

            var fechamento = await _context.Fechamento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fechamento == null)
            {
                return NotFound();
            }

            return View(fechamento);
        }

        // POST: Fechamentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fechamento == null)
            {
                return Problem("Entity set 'PDVContext.Fechamento'  is null.");
            }
            var fechamento = await _context.Fechamento.FindAsync(id);
            if (fechamento != null)
            {
                _context.Fechamento.Remove(fechamento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FechamentoExists(int id)
        {
          return (_context.Fechamento?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
