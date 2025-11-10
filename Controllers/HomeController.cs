using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PDV.Data;
using PDV.Models;
using PDV.Models.ViewModels;
using System.Diagnostics;

namespace PDV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PDVContext _context;

        public HomeController(ILogger<HomeController> logger, PDVContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel();
            homeViewModel.Categorias = _context.Categoria.ToList();
            homeViewModel.Produtos = _context.Produto.ToList();
            ViewBag.TipoPagamentos = _context.TipoPagamento.ToList();
            return View(homeViewModel);
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
