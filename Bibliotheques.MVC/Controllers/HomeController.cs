using Microsoft.AspNetCore.Mvc;
using Bibliotheques.ApplicationCore.Entities;
using Bibliotheques.ApplicationCore.Interfaces;

namespace Bibliotheques.MVC.Controllers
{
    /// <summary>
    /// Contrôleur principal de l'application
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IRepository<Livre> _livreRepository;
        private readonly IRepository<Usager> _usagerRepository;
        private readonly HttpClient _httpClient;

        public HomeController(
            IRepository<Livre> livreRepository,
            IRepository<Usager> usagerRepository,
            IHttpClientFactory httpClientFactory)
        {
            _livreRepository = livreRepository;
            _usagerRepository = usagerRepository;
            _httpClient = httpClientFactory.CreateClient("BibliothequeAPI");
        }

        /// <summary>
        /// Page d'accueil avec statistiques
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var stats = new DashboardStats
                {
                    TotalLivres = await _livreRepository.CountAsync(),
                    TotalUsagers = await _usagerRepository.CountAsync(),
                    LivresDisponibles = (await _livreRepository.GetAllAsync()).Sum(l => l.QuantiteDisponible)
                };

                // Récupérer les statistiques des emprunts via l'API
                var empruntsResponse = await _httpClient.GetAsync("emprunts");
                if (empruntsResponse.IsSuccessStatusCode)
                {
                    var json = await empruntsResponse.Content.ReadAsStringAsync();
                    var emprunts = System.Text.Json.JsonSerializer.Deserialize<List<Emprunt>>(json, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (emprunts != null)
                    {
                        stats.PretsActifs = emprunts.Count(e => e.EstActif);
                        stats.PretsEnRetard = emprunts.Count(e => e.EstEnRetard);
                    }
                }

                return View(stats);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors du chargement des statistiques: {ex.Message}";
                return View(new DashboardStats());
            }
        }

        /// <summary>
        /// Page de confidentialité
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Page d'erreur
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }

    /// <summary>
    /// Modèle pour les statistiques du tableau de bord
    /// </summary>
    public class DashboardStats
    {
        public int TotalLivres { get; set; }
        public int TotalUsagers { get; set; }
        public int PretsActifs { get; set; }
        public int PretsEnRetard { get; set; }
        public int LivresDisponibles { get; set; }
    }
}
