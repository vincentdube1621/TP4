using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Bibliotheques.ApplicationCore.Entities;
using Bibliotheques.ApplicationCore.Interfaces;

namespace Bibliotheques.MVC.Controllers
{
    /// <summary>
    /// Contrôleur MVC pour la gestion des emprunts (consomme l'API)
    /// </summary>
    public class EmpruntsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IRepository<Livre> _livreRepository;
        private readonly IRepository<Usager> _usagerRepository;

        public EmpruntsController(
            IHttpClientFactory httpClientFactory,
            IRepository<Livre> livreRepository,
            IRepository<Usager> usagerRepository)
        {
            _httpClient = httpClientFactory.CreateClient("BibliothequeAPI");
            _livreRepository = livreRepository;
            _usagerRepository = usagerRepository;
        }

        /// <summary>
        /// Affiche la liste des emprunts
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("emprunts");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var emprunts = JsonSerializer.Deserialize<List<Emprunt>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return View(emprunts);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors du chargement des emprunts: {ex.Message}";
            }

            return View(new List<Emprunt>());
        }

        /// <summary>
        /// Affiche les détails d'un emprunt
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"emprunts/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var emprunt = JsonSerializer.Deserialize<Emprunt>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return View(emprunt);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors du chargement de l'emprunt: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Affiche le formulaire de création d'emprunt
        /// </summary>
        public async Task<IActionResult> Create()
        {
            var livres = await _livreRepository.GetAllAsync();
            var usagers = await _usagerRepository.GetAllAsync();

            ViewData["LivreId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                livres.Where(l => l.EstDisponible), "Id", "TitreComplet");
            ViewData["UsagerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                usagers.Where(u => u.PeutEmprunter), "Id", "NomComplet");

            return View();
        }

        /// <summary>
        /// Crée un nouvel emprunt via l'API
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsagerId,LivreId")] CreerEmpruntRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(request);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    
                    var response = await _httpClient.PostAsync("emprunts", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Emprunt créé avec succès";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Erreur lors de la création: {errorContent}";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Erreur lors de la création: {ex.Message}";
                }
            }

            // Recharger les listes en cas d'erreur
            var livres = await _livreRepository.GetAllAsync();
            var usagers = await _usagerRepository.GetAllAsync();

            ViewData["LivreId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                livres.Where(l => l.EstDisponible), "Id", "TitreComplet", request.LivreId);
            ViewData["UsagerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                usagers.Where(u => u.PeutEmprunter), "Id", "NomComplet", request.UsagerId);

            return View(request);
        }

        /// <summary>
        /// Retourne un livre emprunté
        /// </summary>
        public async Task<IActionResult> Retourner(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"emprunts/{id}/retour", null);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Livre retourné avec succès";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Erreur lors du retour: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors du retour: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Supprime un emprunt
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"emprunts/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Emprunt supprimé avec succès";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Erreur lors de la suppression: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors de la suppression: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Affiche les emprunts en retard
        /// </summary>
        public async Task<IActionResult> EnRetard()
        {
            try
            {
                var response = await _httpClient.GetAsync("emprunts/en-retard");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var emprunts = JsonSerializer.Deserialize<List<Emprunt>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return View(emprunts);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors du chargement: {ex.Message}";
            }

            return View(new List<Emprunt>());
        }

        /// <summary>
        /// Affiche les emprunts actifs
        /// </summary>
        public async Task<IActionResult> Actifs()
        {
            try
            {
                var response = await _httpClient.GetAsync("emprunts/actifs");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var emprunts = JsonSerializer.Deserialize<List<Emprunt>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return View(emprunts);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors du chargement: {ex.Message}";
            }

            return View(new List<Emprunt>());
        }
    }

    /// <summary>
    /// Modèle pour créer un emprunt
    /// </summary>
    public class CreerEmpruntRequest
    {
        public int UsagerId { get; set; }
        public int LivreId { get; set; }
    }
}
