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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarioEmpresa == null)
            {
                return NotFound();
            }

            return View(usuarioEmpresa);
        }
        public async Task<IActionResult> ValidaInformacoes()
        {
            // Usamos FirstOrDefaultAsync para pegar o objeto real (precisamos do ID para o Details)
            var empresa = await _context.UsuarioEmpresa.FirstOrDefaultAsync();

            if (empresa == null)
            {
                // Se não tem registro, redireciona para a rota /UsuarioEmpresa/Create
                return RedirectToAction(nameof(Create));
            }
            else
            {
                // Se tem registro, redireciona para a rota /UsuarioEmpresa/Details/5
                return RedirectToAction(nameof(Details), new { id = empresa.Id });
            }
        }

        // GET: UsuarioEmpresa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UsuarioEmpresa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomeRazao,NomeFantasia,CNPJ,InscricaoEstadual,Email,Contato")] UsuarioEmpresa usuarioEmpresa)
        {
            ChaveAcesso chave = new ChaveAcesso
            {
                UUID = "123456",
                DataInicio = DateTime.Now,
                DataFim = DateTime.Now.AddYears(1)
            };
            _context.Add(chave);
            await _context.SaveChangesAsync();

            if (ModelState.IsValid)
            {
                usuarioEmpresa.CNPJ = usuarioEmpresa.CNPJ.Replace(".", "").Replace(".", "").Replace("/", "").Replace("-", "");

                // correção paliativa no momento até alterar o BD
                usuarioEmpresa.Senha = "";
                usuarioEmpresa.ChaveAcessoId = chave.Id;
                _context.Add(usuarioEmpresa);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = usuarioEmpresa.Id });
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeRazao,NomeFantasia,CNPJ,InscricaoEstadual,Email,Contato,DataEntrada")] UsuarioEmpresa usuarioEmpresa)
        {
            if (id != usuarioEmpresa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    usuarioEmpresa.ChaveAcessoId = 2;
                    usuarioEmpresa.Senha = "";
                    usuarioEmpresa.CNPJ = usuarioEmpresa.CNPJ.Replace(".", "").Replace(".", "").Replace("/", "").Replace("-", "");
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
                return RedirectToAction(nameof(Details), new { id = usuarioEmpresa.Id });
            }
            ViewData["ChaveAcessoId"] = new SelectList(_context.ChaveAcesso, "Id", "UUID", usuarioEmpresa.ChaveAcessoId);
            return RedirectToAction(nameof(Details), new { id = usuarioEmpresa.Id });
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
