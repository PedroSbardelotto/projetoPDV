using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
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
        public async Task<IActionResult> Details()
        {
            NotaFiscalViewModel notaFiscalView = new NotaFiscalViewModel();

            notaFiscalView.ListaFilaFaturamentoDetails = new List<NotaFiscalViewModel>();

            List<NotaFiscal> lstNotafiscalPendente = await _context.NotaFiscal.Where(x => x.Status == "PENDENTE").Include(x => x.Produtos).ToListAsync();

            foreach(var item in lstNotafiscalPendente)
            {
                var objFornecedor = await _context.Fornecedor.Where(x => x.Id == item.CodigoFornecedor).FirstOrDefaultAsync();

                var listaFila = new List<FilaFaturamento>();

                foreach (var prod in item.Produtos)
                {
                    listaFila.Add(new FilaFaturamento
                    {
                        CodigoFornecedor = item.CodigoFornecedor,
                        CodigoNFe = Convert.ToInt32(item.Numero),
                        CodigoProduto = prod.Id,
                        Quantidade = prod.Quantidade,
                        Custo = prod.Preco,
                        ValorTotal = prod.ValorTotal
                    });
                }

                notaFiscalView.ListaFilaFaturamentoDetails.Add(new NotaFiscalViewModel
                {
                    Id = item.Id,
                    DataEmissao = item.DataEmissao,
                    Numero = item.Numero,
                    CodigoFornecedor = objFornecedor.Id,
                    NomeFornecedor = objFornecedor.NomeRazao,
                    ValorTotal = item.ValorTotal,
                    Status = item.Status,
                    ListaFilaFaturamento = listaFila
                });
            }

            return View(notaFiscalView);
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
        public async Task<IActionResult> Edit(NotaFiscalViewModel notaFiscalViewModel)
        {

            return View("Edit", notaFiscalViewModel);
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
                TempData["Aviso"] = "Fornecedor não cadatrado.";
                return RedirectToAction(nameof(Index));
            }

            NotaFiscal objNFe = await _context.NotaFiscal.Where(x => x.Numero == ide.nNF).FirstOrDefaultAsync();

            if (objFornecedor != null)
            {
                TempData["Aviso"] = "Este XML já foi importado.";
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
            nfe.Status = "PENDENTE";
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

            await _context.NotaFiscal.AddAsync(nfe);
            await _context.SaveChangesAsync();

            int CodigoNFe = nfe.Id;

            List<ProdutoFornecedor> lstProdutoCadastro = notaFiscalViewModel.ListaProdutos.Where(x => x.ProdutoNovo == true).Select(x => new ProdutoFornecedor
                                    {
                                        CodigoNF = CodigoNFe,
                                        CodProdNF = x.CodigoProdutoNFe,
                                        CodigoFornecedor = notaFiscalViewModel.CodigoFornecedor,
                                        ProdutoId = x.Produto.Id
                                    }).ToList();

            await _context.ProdutoFornecedor.AddRangeAsync(lstProdutoCadastro);
            await _context.SaveChangesAsync();

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
                    CustoAntigo = objProduto.Custo.ToString(),
                    NomeCategoria = objProduto.Categoria.Nome,
                    DataAtualizacao = objProduto.DataAtualizacao.ToString("dd/MM/yyyy")
                });
            }

            notaFiscalViewModel.Id = CodigoNFe;

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

            NotaFiscal objNotaFiscal = await _context.NotaFiscal.FindAsync(notaFiscalViewModel.Id);

            objNotaFiscal.Status = "FATURADA";
            _context.NotaFiscal.Update(objNotaFiscal);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Produtos alterados com  Sucesso";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> FaturamentoPendente(NotaFiscalViewModel notaFiscalViewModel)
        {

            TempData["Aviso"] = "Produtos não faturados!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> FaturarPendente(NotaFiscalViewModel notaFiscalViewModel)
        {
            NotaFiscalViewModel notaFiscalView = new NotaFiscalViewModel();

            notaFiscalView.ListaProdutosFaturamento = new List<ProdutoFaturamento>();
            List<ProdutoFaturamento> lstProdFaturamento = new List<ProdutoFaturamento>();

            NotaFiscal objNFe = await _context.NotaFiscal.Where(x => x.Id == notaFiscalViewModel.Id).Include(x => x.Produtos).FirstOrDefaultAsync();
            Fornecedor objFornecedor = await _context.Fornecedor.Where(x => x.Id == objNFe.CodigoFornecedor).FirstOrDefaultAsync();

            foreach(var item in objNFe.Produtos)
            {
                Produto objProduto = await _context.Produto.Include(x => x.Categoria).Where(x => x.Id == item.CodigoProduto).FirstOrDefaultAsync();

                lstProdFaturamento.Add(new ProdutoFaturamento
                {
                    CodigoProduto = item.CodigoProduto.ToString(),
                    NomeProduto = objProduto.Nome,
                    Quantidade = item.Quantidade.ToString("F2", CultureInfo.InvariantCulture),
                    QuantidadeEstoque = objProduto.Quantidade.ToString(),
                    Custo = item.Preco.ToString(),
                    PrecoVenda = objProduto.Preco.ToString(),
                    ValorTotal = item.ValorTotal.ToString(),
                    CustoAntigo = objProduto.Preco.ToString(),
                    NomeCategoria = objProduto.Categoria.Nome,
                    DataAtualizacao = objProduto.DataAtualizacao.ToString("dd/MM/yyyy")
                });
            }

            notaFiscalView.CodigoFornecedor = objNFe.CodigoFornecedor;
            notaFiscalView.Numero = objNFe.Numero;
            notaFiscalView.DataEmissao = objNFe.DataEmissao;
            notaFiscalView.NomeFornecedor = objFornecedor.NomeRazao;
            notaFiscalView.ListaProdutosFaturamento = lstProdFaturamento;

            return View("Edit", notaFiscalView);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarNotasPartial(string Fornecedor, DateTime? DataInicial, string Numero)
        {

            var resultados = await _context.NotaFiscal
                // ... Where, Filtros, etc ...
                .Select(n => new NotaFiscalViewModel // Projeta para o DTO
                {
                    Id = n.Id,
                    Numero = n.Numero,
                    DataEmissao = n.DataEmissao, // Deixe o Razor formatar a data
                    ValorTotal = n.ValorTotal,
                    Status = n.Status
                })
                .ToListAsync();

            // 2. RETORNA A PARTIAL VIEW (o HTML já renderizado)
            return PartialView("_BuscarNFe", resultados);
        }

        public async Task<IActionResult> BuscaEPopulaNota(NotaFiscalViewModel notaFiscalViewModel)
        {
            NotaFiscal objNFe = await _context.NotaFiscal.Where(x => x.Id == notaFiscalViewModel.Id).Include(x => x.Produtos).FirstOrDefaultAsync();
            Fornecedor objFornecedor = await _context.Fornecedor.Where(x => x.Id == objNFe.CodigoFornecedor).FirstOrDefaultAsync();

            notaFiscalViewModel.ChaveAcesso = objNFe.ChaveAcesso;
            notaFiscalViewModel.Numero = objNFe.Numero;
            notaFiscalViewModel.DataEmissao = objNFe.DataEmissao;
            notaFiscalViewModel.DataEntrada = objNFe.DataEntrada;
            notaFiscalViewModel.CNPJ = objFornecedor.CNPJ;
            notaFiscalViewModel.CodigoFornecedor = objFornecedor.Id;
            notaFiscalViewModel.InscricaoEstadual = objFornecedor.InscricaoEstadual;
            notaFiscalViewModel.NomeFornecedor = objFornecedor.NomeRazao;
            notaFiscalViewModel.ValorTotal = objNFe.ValorTotal;
            notaFiscalViewModel.Serie = objNFe.Serie.ToString();
            notaFiscalViewModel.NaturesaOperacao = objNFe.NatuOp;
            notaFiscalViewModel.ListaProdutos = new List<ProdutoNFe>();

            foreach(var item in objNFe.Produtos)
            {
                notaFiscalViewModel.ListaProdutos.Add(new ProdutoNFe
                {
                    CodigoProdutoNFe = item.CodigoProdNFe,
                    NomeProdutoNFe = item.Nome,
                    QuantidadeInterna = item.Quantidade,
                    PrecoProdutoNFe = item.Preco,
                    UnidadeMedida = item.UnidadeMedidaNFe,
                    ValorTotalProdNFe = item.ValorTotal.ToString()
                });
            }

            notaFiscalViewModel.AbrirModal = false;
            notaFiscalViewModel.BotaoGravar = false;
            notaFiscalViewModel.BotaoCancelar = true;

            return View("index", notaFiscalViewModel);
        }

        public async Task<IActionResult> CancelarNFe(NotaFiscalViewModel notaFiscalViewModel)
        {
            NotaFiscal objNFe = await _context.NotaFiscal.Where(x => x.Id == notaFiscalViewModel.Id).Include(x => x.Produtos).FirstOrDefaultAsync();

            if(objNFe.Status == "FATURADA")
            {
                foreach (var item in objNFe.Produtos)
                {
                    Produto objProduto = await _context.Produto.FirstOrDefaultAsync(x => x.Id == Convert.ToUInt32(item.CodigoProduto));

                    objProduto.Quantidade -= (int)Convert.ToDecimal(item.Quantidade, System.Globalization.CultureInfo.InvariantCulture);

                    _context.Produto.Update(objProduto);
                    await _context.SaveChangesAsync();
                }

                _context.NotaFiscal.Remove(objNFe);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "NFe cancelda com sucesso!";
                return View("index", notaFiscalViewModel);
            }

            TempData["Erro"] = "NFe pendente de faturamento. impossivel excluir!";
            return RedirectToAction("Details", "NotaFiscal");

        }
    }
}
