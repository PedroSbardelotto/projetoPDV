using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NFeDANFEGenerator;
using PDV.Data;
using PDV.Enums;
using PDV.Models;
using PDV.Models.ViewModels;
using System.Globalization;
using System.Linq;

namespace PDV.Controllers
{
    public class NotaFiscalController : Controller
    {
        private readonly PDVContext _context;

        public NotaFiscalController(PDVContext context)
        {
            _context = context;
        }

        // GET: Vendas
        public async Task<IActionResult> Index(int? pageNumber)
        {
            NotaFiscalViewModel notaFiscalView = new NotaFiscalViewModel();

            return View(notaFiscalView);
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
                .Include(v => v.ProdutosVenda)
                    .ThenInclude(pv => pv.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendas == null)
            {
                return NotFound();
            }

            var somaValores = vendas.ProdutosVenda.Sum(x => x.Produto.Preco * x.Quantidade);

            if (vendas.ValorTotal != somaValores)
            {
                var novoValor = somaValores - vendas.ValorTotal;
                vendas.ValorTotal = somaValores - novoValor;
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

            // ADICIONEI OS INCLUDES PARA TRAZER OS DADOS DE VISUALIZAÇÃO
            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.TipoPagamento)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vendas == null)
            {
                return NotFound();
            }

            // Apenas o Tipo de Pagamento precisa ser um Dropdown selecionável
            ViewData["TipoPagamentoId"] = new SelectList(_context.TipoPagamento, "Id", "Descricao", vendas.TipoPagamentoId);

            // Removemos os outros ViewDatas que estavam causando o erro ou eram desnecessários para essa etapa

            return View(vendas);
        }

        // POST: Vendas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ValorTotal,Status,DataEntrada,DataAtualizacao,TipoPagamentoId,ClienteId,FechamentoId")] Vendas vendas)
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

        public async Task<IActionResult> Comprovante(int? id)
        {
            if (id == null) return NotFound();
            var venda = await _context.Vendas.Include(v => v.Cliente).Include(v => v.TipoPagamento).FirstOrDefaultAsync(m => m.Id == id);

            if (venda == null) return NotFound();
            return View(venda);
        }

        private bool VendasExists(int id)
        {
            return (_context.Vendas?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> LerXML([Bind("arquivoXML")] NotaFiscalViewModel notaFiscalViewModel)
        {
            if(notaFiscalViewModel.arquivoXML == null)
                return NotFound();

            string xmlContent;

            using (var stream = notaFiscalViewModel.arquivoXML.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                xmlContent = await reader.ReadToEndAsync();
            }

            var notaFiscal = NFeService.DeserializeNFeXML(xmlContent);

            var infNFe = notaFiscal.NFe.infNFe;
            var ide = infNFe.ide;
            var emit = infNFe.emit;
            var total = infNFe.total;

            Fornecedor objFornecedor = await _context.Fornecedor.Where(x => x.CNPJ == emit.CNPJ).FirstOrDefaultAsync();

            if (objFornecedor == null)
            {
                TempData["Erro"] = "Fornecedor não cadatrado.";
                return RedirectToAction(nameof(Index));
            }

            List<ProdutoFornecedor> lstProdutoFornecedor = await _context.ProdutoFornecedor.Include(x => x.Produto).Include(x => x.Produto.Categoria).Where(x => x.CodigoFornecedor == objFornecedor.Id).ToListAsync();

            foreach (var item in infNFe.det)
            {
                var prod = item.prod;

                string parteInteira = prod.qCom.Split('.')[0];

                ProdutoFornecedor objProdutoExiste = lstProdutoFornecedor.FirstOrDefault(x => x.CodProdNF == prod.cProd);

                if(objProdutoExiste != null)
                {
                    notaFiscalViewModel.ListaProdutos.Add(new ProdutoNFe
                    {
                        CodigoProdutoNFe = prod.cProd,
                        NomeProdutoNFe = prod.xProd,
                        QuantidadeNFe = decimal.Parse(prod.qCom, CultureInfo.InvariantCulture),
                        QuantidadeInterna = decimal.Parse(prod.qCom, CultureInfo.InvariantCulture),
                        PrecoProdutoNFe = decimal.Parse(prod.vUnCom, CultureInfo.InvariantCulture),
                        ValorTotalProdNFe = prod.vProd,
                        Produto = objProdutoExiste.Produto,
                        UnidadeMedida = prod.uCom,
                        Validado = true,
                        ProdutoNovo = false
                    });
                } else
                {
                    notaFiscalViewModel.ListaProdutos.Add(new ProdutoNFe
                    {
                        CodigoProdutoNFe = prod.cProd,
                        NomeProdutoNFe = prod.xProd,
                        QuantidadeNFe = decimal.Parse(prod.qCom, CultureInfo.InvariantCulture),
                        QuantidadeInterna = decimal.Parse(prod.qCom, CultureInfo.InvariantCulture),
                        PrecoProdutoNFe = decimal.Parse(prod.vUnCom, CultureInfo.InvariantCulture),
                        ValorTotalProdNFe = prod.vProd,
                        UnidadeMedida = prod.uCom,
                        Validado = false,
                        ProdutoNovo = true
                    });
                }
            }

            notaFiscalViewModel.CodigoFornecedor = objFornecedor.Id;
            notaFiscalViewModel.ListaProdutosCadastrados = await _context.Produto.Where(x => x.Status == true).ToListAsync();
            notaFiscalViewModel.ConteudoXML = xmlContent;
            notaFiscalViewModel.AbrirModal = true;

            //TempData["Sucesso"] = "nota importada com sucesso";
            //return View(notaFiscalViewModel);
            return View("index", notaFiscalViewModel);
        }

        public async Task<IActionResult> PopularDadosNFE(NotaFiscalViewModel notaFiscalViewModel)
        {
            var notaFiscal = NFeService.DeserializeNFeXML(notaFiscalViewModel.ConteudoXML);

            var infNFe = notaFiscal.NFe.infNFe;
            var ide = infNFe.ide;
            var emit = infNFe.emit;
            var total = infNFe.total;

            notaFiscalViewModel.ChaveAcesso = notaFiscal.protNFe.infProt.chNFe;
            notaFiscalViewModel.Numero = ide.nNF;
            notaFiscalViewModel.DataEmissao = Convert.ToDateTime(ide.dhEmi);
            notaFiscalViewModel.CNPJ = emit.CNPJ;
            notaFiscalViewModel.InscricaoEstadual = emit.IE;
            notaFiscalViewModel.NomeFornecedor = emit.xNome;
            notaFiscalViewModel.ValorTotal = decimal.Parse(total.ICMSTot.vNF, CultureInfo.InvariantCulture);
            notaFiscalViewModel.Serie = ide.serie;
            notaFiscalViewModel.NaturesaOperacao = ide.natOp;

            foreach(var item in notaFiscalViewModel.ListaProdutos)
            {
                if(item.QuantidadeInterna != item.QuantidadeNFe)
                {
                    item.PrecoProdutoNFe = decimal.Parse(item.ValorTotalProdNFe, CultureInfo.InvariantCulture) / item.QuantidadeInterna;
                }
            }

            notaFiscalViewModel.AbrirModal = false;
            notaFiscalViewModel.BotaoGravar = true;

            return View("index", notaFiscalViewModel);
        }
        public async Task<IActionResult> GravarNFe(NotaFiscalViewModel notaFiscalViewModel)
        {
            NotaFiscal nfe = new NotaFiscal();

            nfe.ChaveAcesso = notaFiscalViewModel.ChaveAcesso;
            nfe.DataEmissao = (DateTime)notaFiscalViewModel.DataEmissao;
            nfe.Numero = notaFiscalViewModel.Numero;
            nfe.CodigoFornecedor = notaFiscalViewModel.CodigoFornecedor;
            nfe.ValorTotal = notaFiscalViewModel.ValorTotal;
            nfe.DataEntrada = DateTime.Now;
            nfe.Serie = Convert.ToInt32(notaFiscalViewModel.Serie);
            nfe.NatuOp = notaFiscalViewModel.NaturesaOperacao;
            nfe.Produtos = notaFiscalViewModel.ListaProdutos.Select(x => new ProdutosNFe
            {
                CodigoProdNFe = x.CodigoProdutoNFe,
                Nome = x.NomeProdutoNFe,
                Quantidade = Convert.ToInt32(x.QuantidadeInterna),
                QuantidadeNFe = x.QuantidadeNFe,
                UnidadeMedidaNFe = x.UnidadeMedida,
                Preco = x.PrecoProdutoNFe,
                ValorTotal = decimal.Parse(x.ValorTotalProdNFe, CultureInfo.InvariantCulture),
                CodigoProduto = x.Produto.Id
            }).ToList();

            //await _context.NotaFiscal.AddAsync(nfe);
            //await _context.SaveChangesAsync();

            int CodigoNFe = nfe.Id;

            List<ProdutoFornecedor> lstProdutoCadastro = notaFiscalViewModel.ListaProdutos.Where(x => x.ProdutoNovo == true).Select(x => new ProdutoFornecedor
                                    {
                                        CodigoNF = CodigoNFe,
                                        CodProdNF = x.CodigoProdutoNFe,
                                        CodigoFornecedor = notaFiscalViewModel.CodigoFornecedor,
                                        ProdutoId = x.Produto.Id
                                    }).ToList();

            //await _context.ProdutoFornecedor.AddRangeAsync(lstProdutoCadastro);
            //await _context.SaveChangesAsync();

            notaFiscalViewModel.ListaProdutosFaturamento = new List<ProdutoFaturamento>();

            foreach(var item in notaFiscalViewModel.ListaProdutos)
            {
                Produto objProduto = await _context.Produto.Include(x => x.Categoria).Where(x=> x.Id == item.Produto.Id).FirstOrDefaultAsync();

                notaFiscalViewModel.ListaProdutosFaturamento.Add(new ProdutoFaturamento
                {
                    CodigoProduto = objProduto.Id.ToString(),
                    NomeProduto = objProduto.Nome,
                    Quantidade = item.QuantidadeNFe.ToString("F2", CultureInfo.InvariantCulture),
                    QuantidadeEstoque = objProduto.Quantidade.ToString(),
                    Custo = item.PrecoProdutoNFe.ToString(),
                    PrecoVenda = objProduto.Preco.ToString(),
                    ValorTotal = item.ValorTotalProdNFe,
                    CustoAntigo = objProduto.Preco.ToString(),
                    NomeCategoria = objProduto.Categoria.Nome,
                    DataAtualizacao = objProduto.DataAtualizacao.ToString("dd/MM/yyyy")
                });
            }

            return View("Edit", notaFiscalViewModel);
        }

        public async Task<IActionResult> AlteraProdutos(NotaFiscalViewModel notaFiscalViewModel)
        {


            foreach(var item in notaFiscalViewModel.ListaProdutosFaturamento)
            {
                Produto objProduto = await _context.Produto.FirstOrDefaultAsync(x => x.Id == Convert.ToUInt32(item.CodigoProduto));

                objProduto.Quantidade += (int)Convert.ToDecimal(item.Quantidade, System.Globalization.CultureInfo.InvariantCulture);
                objProduto.Custo = Convert.ToDecimal(item.Custo);
                objProduto.DataAtualizacao = DateTime.Now;

                if (objProduto.Preco != Convert.ToDecimal(item.PrecoVenda))
                    objProduto.Preco = Convert.ToDecimal(item.PrecoVenda);

                _context.Produto.Update(objProduto);
                await _context.SaveChangesAsync();
            }

            TempData["Sucesso"] = "Produtos alterados com  Sucesso";
            return RedirectToAction(nameof(Index));
        }
    }
}
