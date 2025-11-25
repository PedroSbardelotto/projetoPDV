using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDV.Data;
using PDV.Models;

namespace PDV.Controllers
{
    public class FornecedorsController : Controller
    {
        private readonly PDVContext _context;

        public FornecedorsController(PDVContext context)
        {
            _context = context;
        }

        // GET: Fornecedors
        // Alterado para receber o termo de pesquisa
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var fornecedores = from f in _context.Fornecedor
                               select f;

            if (!String.IsNullOrEmpty(searchString))
            {
                // Filtra por Nome Fantasia, Razão Social ou CNPJ
                fornecedores = fornecedores.Where(s => s.NomeFantasia.Contains(searchString)
                                                    || s.NomeRazao.Contains(searchString)
                                                    || s.CNPJ.Contains(searchString));
            }

            return View(await fornecedores.AsNoTracking().ToListAsync());
        }

        // GET: Fornecedors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fornecedor == null)
            {
                return NotFound();
            }

            var fornecedor = await _context.Fornecedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            return View(fornecedor);
        }

        // GET: Fornecedors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fornecedors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // REMOVIDO DataEntrada e DataAtualizacao do Bind para evitar erros de validação
        public async Task<IActionResult> Create([Bind("Id,NomeRazao,NomeFantasia,CNPJ,InscricaoEstadual,Email,Endereco,Contato")] Fornecedor fornecedor)
        {
            // Define os valores do sistema manualmente
            fornecedor.Status = true;
            fornecedor.DataEntrada = DateTime.Now;
            fornecedor.DataAtualizacao = DateTime.Now;

            if (ModelState.IsValid)
            {
                fornecedor.CNPJ = fornecedor.CNPJ.Replace(".", "").Replace(".", "").Replace("/", "").Replace("-", "");
                _context.Add(fornecedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fornecedor);
        }

        // GET: Fornecedors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fornecedor == null)
            {
                return NotFound();
            }

            var fornecedor = await _context.Fornecedor.FindAsync(id);
            if (fornecedor == null)
            {
                return NotFound();
            }
            return View(fornecedor);
        }

        // POST: Fornecedors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ATENÇÃO: Adicionei "Status" na lista do Bind abaixo
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeRazao,NomeFantasia,CNPJ,InscricaoEstadual,Email,Endereco,Contato,Status,DataEntrada")] Fornecedor fornecedor)
        {
            if (id != fornecedor.Id)
            {
                return NotFound();
            }

            // Garante que a data de atualização seja a de agora
            fornecedor.DataAtualizacao = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fornecedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FornecedorExists(fornecedor.Id))
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
            return View(fornecedor);
        }

        // GET: Fornecedors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fornecedor == null)
            {
                return NotFound();
            }

            var fornecedor = await _context.Fornecedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            return View(fornecedor);
        }

        // POST: Fornecedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fornecedor == null)
            {
                return Problem("Entity set 'PDVContext.Fornecedor'  is null.");
            }
            var fornecedor = await _context.Fornecedor.FindAsync(id);
            if (fornecedor != null)
            {
                _context.Fornecedor.Remove(fornecedor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FornecedorExists(int id)
        {
            return (_context.Fornecedor?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        // POST: Fornecedors/DeleteOrActivate/5
        [HttpPost, ActionName("DeleteOrActivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrActivateConfirmed(int id)
        {
            if (_context.Fornecedor == null)
            {
                return Problem("Entity set 'PDVContext.Fornecedor' is null.");
            }

            var fornecedor = await _context.Fornecedor.FindAsync(id);

            if (fornecedor != null)
            {
                // Inverte o status (Toggle): Se estava Ativo vira Inativo, e vice-versa.
                // Usa GetValueOrDefault() para tratar casos onde o Status possa vir nulo.
                fornecedor.Status = !fornecedor.Status.GetValueOrDefault();

                // Atualizamos a data de modificação para manter o histórico correto
                fornecedor.DataAtualizacao = DateTime.Now;

                _context.Fornecedor.Update(fornecedor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}