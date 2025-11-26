using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PDV.Data;
using PDV.Models;
using PDV.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                        View(await _context.ProdutoPromocao
                        .Include(p => p.Produto).OrderBy(p => p.Produto.Nome).ToListAsync()) :
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

            var produtoPromocao = await _context.ProdutoPromocao.Include(p => p.Produto).FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Preco,DataInicio,DataFim, Produto, ProdutoId")] ProdutoPromocao produtoPromocao)
        {
            if (id != produtoPromocao.Id)
            {
                return NotFound();
            }

            var produto = await _context.Produto.FindAsync(produtoPromocao.ProdutoId);

            produtoPromocao.Produto = produto;

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
                TempData["Erro"] = "Produto não encontrado!";
                return Problem("Entity set 'PDVContext.ProdutoPromocao'  is null.");
            }
            var produtoPromocao = await _context.ProdutoPromocao.FindAsync(id);
            if (produtoPromocao != null)
            {
                _context.ProdutoPromocao.Remove(produtoPromocao);
            }

            TempData["Sucesso"] = "Produto excluído com sucesso!";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoPromocaoExists(int id)
        {
            return (_context.ProdutoPromocao?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> SalvarPromocao([FromBody] PromocaoSalvarViewModel model)
        {
            if (model.Produtos == null || !model.Produtos.Any())
            {
                TempData["Erro"] = "Nenhum produto Selecionado!";
            }

            // Lógica para salvar cada item da lista no banco
            var promocoesParaSalvar = model.Produtos.Select(item => new ProdutoPromocao
            {
                ProdutoId = item.ProdutoId,
                Preco = item.Preco,
                DataInicio = item.DataInicio,
                DataFim = item.DataFim,
                DataEntrada = DateTime.Now
            }).ToList();

            _context.ProdutoPromocao.AddRange(promocoesParaSalvar);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Promoções adicionadas!";
            return Json(new { sucesso = true, mensagem = "" });

        }

        [HttpGet]
        public async Task<IActionResult> BuscarProdutos(string termo)
        {
            // Use ToLower() para busca case-insensitive no servidor (melhor performance no banco)
            var produtos = await _context.Produto
                .Where(p => string.IsNullOrEmpty(termo) || p.Nome.Contains(termo) || p.Id.ToString() == termo)
                .Select(p => new
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Preco = p.Preco,
                    Custo = p.Custo,
                    // Adicione outras propriedades necessárias
                })
                .Take(50) // Limita o número de resultados para performance
                .ToListAsync();

            return Json(produtos);
        }
    }
}
