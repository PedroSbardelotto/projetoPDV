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
using PDV.Models.ViewModels;

namespace PDV.Controllers
{
    public class ClientesController : Controller
    {
        private readonly PDVContext _context;

        public ClientesController(PDVContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
              return _context.Cliente != null ? 
                          View(await _context.Cliente.ToListAsync()) :
                          Problem("Entity set 'PDVContext.Cliente'  is null.");
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cliente == null)
            {
                return NotFound();
            }
            ClienteDetailsViewModel ClienteVM = new ClienteDetailsViewModel();
            ClienteVM.Cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ClienteVM.Cliente == null)
            {
                return NotFound();
            }

            ClienteVM.Pendencias = await _context.Vendas.Where(v => v.ClienteId == ClienteVM.Cliente.Id).Where(v=> v.Status != 1).ToListAsync();
            ViewBag.TipoPagamentos = await _context.TipoPagamento.ToListAsync();
            return View(ClienteVM);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Email,Contato,CPF")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.CPF = cliente.CPF.Replace(".", "").Replace("-", "");
                cliente.Contato = cliente.Contato.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                cliente.DataEntrada = DateTime.Now;
                cliente.DataAtualizacao = DateTime.Now;

                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cliente == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Contato,CPF,InscricaoEstadual,DataEntrada,DataAtualizacao")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                cliente.CPF = cliente.CPF.Replace(".", "").Replace("-", "");
                cliente.Contato = cliente.Contato.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                cliente.DataAtualizacao = DateTime.Now;
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cliente == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cliente == null)
            {
                return Problem("Entity set 'PDVContext.Cliente' is null.");
            }

            var cliente = await _context.Cliente.FindAsync(id);

            if (cliente != null)
            {
                // 1. VERIFICAÇÃO DE PENDÊNCIAS
                // Verifica se existe QUALQUER venda deste cliente que não esteja finalizada (Status != 1)
                // Usei AnyAsync para performance (para no primeiro que encontrar)
                bool possuiPendencias = await _context.Vendas
                    .Where(v => v.ClienteId == id)
                    .Where(v => v.Status == (short)Situacao.Pendente) 
                    .AnyAsync();

                if (possuiPendencias)
                {
                    // 2. AVISO DE ERRO
                    // Define a mensagem e devolve o usuário para a lista sem apagar nada
                    TempData["Erro"] = $"O cliente {cliente.Nome} possui pendências ativas e não pode ser excluído.";
                    return RedirectToAction(nameof(Index));
                }

                // Se chegou aqui, pode excluir
                _context.Cliente.Remove(cliente);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Cliente excluído com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PagarPendencia(int id, int tipoPagamentoId) 
        {
            // Busca a venda
            Vendas pendencia = await _context.Vendas.FindAsync(id);

            if (pendencia == null)
            {
                TempData["Erro"] = "Pendência não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            pendencia.Status = (short)Situacao.Finalizado;
            pendencia.TipoPagamentoId = tipoPagamentoId;
            pendencia.DataAtualizacao = DateTime.Now;

            try
            {
                _context.Update(pendencia);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Pagamento realizado com sucesso!";
            }
            catch
            {
                TempData["Erro"] = "Erro ao processar pagamento.";
            }

            return RedirectToAction(nameof(Details), new { id = pendencia.ClienteId });
        }
        private bool ClienteExists(int id)
        {
          return (_context.Cliente?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
