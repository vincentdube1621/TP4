using BibliothequeLIPAJOLI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace BibliothequeLIPAJOLI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BibliothequeContext _context;

        public HomeController(ILogger<HomeController> logger, BibliothequeContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stats = new DashboardStats
            {
                TotalLivres = await _context.Livres.CountAsync(),
                TotalUsagers = await _context.Usagers.CountAsync(),
                PretsActifs = await _context.Prets.CountAsync(p => p.DateRetourReelle == null),
                PretsEnRetard = await _context.Prets.CountAsync(p => p.DateRetourReelle == null && p.DateRetourPrevue < DateTime.Now),
                LivresDisponibles = await _context.Livres.SumAsync(l => l.QuantiteDisponible)
            };

            return View(stats);
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
