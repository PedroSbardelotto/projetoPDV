using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PDV.Data;
using PDV.Models;
using PDV.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDV.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly PDVContext _context;

        public ProdutosController(PDVContext context)
        {
            _context = context;
        }

        //// GET: Produtos
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var pdvContext = _context.Produto.Include(p => p.Categoria).AsQueryable();

            // Ordenação é importante para paginação não "pular" itens
            pdvContext = pdvContext.OrderBy(p => p.Nome);

            int pageSize = 10; // Quantidade por página

            // Retorna a lista paginada
            return View(await PaginatedList<Produto>.CreateAsync(pdvContext.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Produto == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome");
            return View();
        }

        // POST: Produtos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Quantidade,QuantidadeMinima,Preco,CategoriaId,Custo")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                produto.Status = true;
                produto.DataEntrada = DateTime.Now;
                produto.DataAtualizacao = DateTime.Now;

                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Produto == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // POST: Produtos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Quantidade,QuantidadeMinima,Preco,Status,DataEntrada,DataAtualizacao,CategoriaId,Custo")] Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    produto.DataAtualizacao = DateTime.Now;
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
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
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Produto == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("DeleteOrActivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrActivateConfirmed(int id)
        {
            if (_context.Produto == null)
            {
                return Problem("Entity set 'PDVContext.Produto'  is null.");
            }
            var produto = await _context.Produto.FindAsync(id);
            if (produto != null)
            {
                produto.Status = produto.Status ? false : true;
                _context.Produto.Update(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProdutoExists(int id)
        {
            return (_context.Produto?.Any(e => e.Id == id)).GetValueOrDefault();
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
