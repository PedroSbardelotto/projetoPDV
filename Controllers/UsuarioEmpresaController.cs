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
    public class UsuarioEmpresaController : Controller
    {
        private readonly PDVContext _context;

        public UsuarioEmpresaController(PDVContext context)
        {
            _context = context;
        }

        // GET: UsuarioEmpresa
        public async Task<IActionResult> Index()
        {
            var pDVContext = _context.UsuarioEmpresa.Include(u => u.ChaveAcesso);
            return View(await pDVContext.ToListAsync());
        }

        // GET: UsuarioEmpresa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsuarioEmpresa == null)
            {
                return NotFound();
            }

            var usuarioEmpresa = await _context.UsuarioEmpresa
                .Include(u => u.ChaveAcesso)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarioEmpresa == null)
            {
                return NotFound();
            }

            return View(usuarioEmpresa);
        }

        // GET: UsuarioEmpresa/Create
        public IActionResult Create()
        {
            ViewData["ChaveAcessoId"] = new SelectList(_context.ChaveAcesso, "Id", "UUID");
            return View();
        }

        // POST: UsuarioEmpresa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomeRazao,NomeFantasia,CNPJ,InscricaoEstadual,Email,Senha,Contato,DataEntrada,ChaveAcessoId")] UsuarioEmpresa usuarioEmpresa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarioEmpresa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChaveAcessoId"] = new SelectList(_context.ChaveAcesso, "Id", "UUID", usuarioEmpresa.ChaveAcessoId);
            return View(usuarioEmpresa);
        }

        // GET: UsuarioEmpresa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UsuarioEmpresa == null)
            {
                return NotFound();
            }

            var usuarioEmpresa = await _context.UsuarioEmpresa.FindAsync(id);
            if (usuarioEmpresa == null)
            {
                return NotFound();
            }
            ViewData["ChaveAcessoId"] = new SelectList(_context.ChaveAcesso, "Id", "UUID", usuarioEmpresa.ChaveAcessoId);
            return View(usuarioEmpresa);
        }

        // POST: UsuarioEmpresa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeRazao,NomeFantasia,CNPJ,InscricaoEstadual,Email,Senha,Contato,DataEntrada,ChaveAcessoId")] UsuarioEmpresa usuarioEmpresa)
        {
            if (id != usuarioEmpresa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioEmpresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioEmpresaExists(usuarioEmpresa.Id))
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
            ViewData["ChaveAcessoId"] = new SelectList(_context.ChaveAcesso, "Id", "UUID", usuarioEmpresa.ChaveAcessoId);
            return View(usuarioEmpresa);
        }

        // GET: UsuarioEmpresa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsuarioEmpresa == null)
            {
                return NotFound();
            }

            var usuarioEmpresa = await _context.UsuarioEmpresa
                .Include(u => u.ChaveAcesso)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarioEmpresa == null)
            {
                return NotFound();
            }

            return View(usuarioEmpresa);
        }

        // POST: UsuarioEmpresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsuarioEmpresa == null)
            {
                return Problem("Entity set 'PDVContext.UsuarioEmpresa'  is null.");
            }
            var usuarioEmpresa = await _context.UsuarioEmpresa.FindAsync(id);
            if (usuarioEmpresa != null)
            {
                _context.UsuarioEmpresa.Remove(usuarioEmpresa);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioEmpresaExists(int id)
        {
          return (_context.UsuarioEmpresa?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
