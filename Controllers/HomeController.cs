using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PDV.Data;
using PDV.Models;
using PDV.Models.Interfaces;
using PDV.Models.ViewModels;
using System.Diagnostics;

namespace PDV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PDVContext _context;
        private readonly IFechamentoService _fechamentoService;

        public HomeController(ILogger<HomeController> logger, PDVContext context, IFechamentoService fechamentoService)
        {
            _logger = logger;
            _context = context;
            _fechamentoService = fechamentoService;
        }

        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel();
            homeViewModel.Categorias = _context.Categoria.ToList();
            homeViewModel.Produtos = _context.Produto.Where(p=> p.Status == true).ToList();
            homeViewModel.Clientes = _context.Cliente.ToList();
            homeViewModel.Fechamento = _context.Fechamento.Where(f => f.DataFechamento == null).SingleOrDefault();
            ViewBag.TipoPagamentos = _context.TipoPagamento.ToList();
            return View(homeViewModel);
        }

        [HttpPost]
        public IActionResult AbrirCaixa(decimal valorInicial)
        {
            bool sucesso = _fechamentoService.AbrirNovoCaixa(valorInicial);
            if (!sucesso)
            {
                TempData["Erro"] = "Já existe um caixa aberto! Encerre o anterior antes de abrir um novo.";
            }
            else
            {
                TempData["Sucesso"] = "Caixa aberto com sucesso!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult FecharCaixa(decimal ValorFechamento)
        {
            bool sucesso = _fechamentoService.FecharCaixa(ValorFechamento);
            if (!sucesso)
            {
                TempData["Erro"] = "Erro para fechar o caixa! Contate um profissional.";
            }
            else
            {
                TempData["Sucesso"] = "Caixa fechado com sucesso!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlesPathFeatures = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            if(exceptionHandlesPathFeatures != null)
            {
                _logger.LogError(exceptionHandlesPathFeatures.Error, "Erro global capturado na rota: {Path}", exceptionHandlesPathFeatures.Path);
            }

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            });
        }
    }
}
