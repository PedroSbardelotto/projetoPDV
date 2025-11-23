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
            var pDVContext = _context.Vendas.Include(v => v.Cliente).Include(v => v.Fechamento).Include(v => v.TipoPagamento).Include(v => v.UsuarioEmpresa).OrderByDescending(v=>v.DataEntrada);
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendas == null)
            {
                return NotFound();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vendas vendas, int[] TipoPagamentoId, string[] ValorPagamentos)
        {
            // 1. Validação do Fechamento (Segurança essencial)
            Fechamento objFechamento = await _context.Fechamento
                                                     .Where(f => f.DataFechamento == null)
                                                     .FirstOrDefaultAsync();

            if (objFechamento == null)
            {
                TempData["Erro"] = "Não é possível realizar venda. O caixa está fechado!";
                return RedirectToAction("Index", "Home");
            }

            DateTime dtAtual = DateTime.Now;

            // Captura os produtos que vieram do formulário (para não perder a referência)
            var listaProdutos = vendas.ProdutosVenda;

            // =================================================================================
            // CENÁRIO A: PENDÊNCIA / FIADO (Sem pagamento imediato)
            // =================================================================================
            if (vendas.ClienteId != null && (TipoPagamentoId == null || TipoPagamentoId.Length == 0))
            {
                if (vendas.ValorTotal <= 0)
                {
                    TempData["Erro"] = "O valor da venda não pode ser zero.";
                    return RedirectToAction("Index", "Home");
                }

                vendas.Status = (short)Situacao.Pendente; // Status 2
                vendas.TipoPagamentoId = null; // Sem pagamento agora
                vendas.FechamentoId = objFechamento.Id;
                vendas.DataEntrada = dtAtual;
                vendas.DataAtualizacao = dtAtual;

                // Mantém os produtos nesta venda única
                vendas.ProdutosVenda = listaProdutos;

                _context.Add(vendas);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Pendência gerada com sucesso!";
            }
            // =================================================================================
            // CENÁRIO B: PAGAMENTO REALIZADO (Um ou Múltiplos Pagamentos)
            // =================================================================================
            else
            {
                if (TipoPagamentoId == null || TipoPagamentoId.Length == 0)
                {
                    TempData["Erro"] = "Selecione pelo menos uma forma de pagamento.";
                    return RedirectToAction("Index", "Home");
                }

                try
                {
                    // Loop para criar uma venda para cada forma de pagamento selecionada
                    for (int i = 0; i < TipoPagamentoId.Length; i++)
                    {
                        Vendas novaVenda = new Vendas();

                        // Dados Comuns
                        novaVenda.ClienteId = vendas.ClienteId;
                        novaVenda.FechamentoId = objFechamento.Id;
                        novaVenda.DataEntrada = dtAtual;
                        novaVenda.DataAtualizacao = dtAtual;
                        novaVenda.Status = (short)Situacao.Finalizado; // Status 1

                        // Dados Específicos do Loop
                        novaVenda.TipoPagamentoId = TipoPagamentoId[i];

                        // Tratamento seguro do valor (string -> decimal)
                        if (ValorPagamentos != null && ValorPagamentos.Length > i)
                        {
                            string valorLimpo = ValorPagamentos[i].Replace(".", ","); // Garante virgula para pt-BR
                            if (decimal.TryParse(valorLimpo, out decimal valorDecimal))
                            {
                                novaVenda.ValorTotal = valorDecimal;
                            }
                        }

                        // --- LÓGICA CRÍTICA DE ESTOQUE ---
                        // Só vinculamos os produtos na PRIMEIRA venda (i == 0).
                        // As outras (i > 0) são apenas financeiras. Se vincularmos em todas,
                        // o estoque será baixado múltiplas vezes (duplicado/triplicado).
                        if (i == 0)
                        {
                            novaVenda.ProdutosVenda = listaProdutos;
                        }
                        else
                        {
                            novaVenda.ProdutosVenda = null;
                        }

                        _context.Add(novaVenda);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Sucesso"] = "Venda finalizada com sucesso!";
                }
                catch (Exception ex)
                {
                    TempData["Erro"] = "Erro ao processar venda: " + ex.Message;
                }
            }

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
