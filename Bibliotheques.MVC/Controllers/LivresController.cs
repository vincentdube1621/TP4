using Microsoft.AspNetCore.Mvc;
using Bibliotheques.ApplicationCore.Entities;
using Bibliotheques.ApplicationCore.Interfaces;

namespace Bibliotheques.MVC.Controllers
{
    /// <summary>
    /// Contrôleur MVC pour la gestion des livres (gestion locale)
    /// </summary>
    public class LivresController : Controller
    {
        private readonly IRepository<Livre> _repository;

        public LivresController(IRepository<Livre> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Affiche la liste des livres
        /// </summary>
        public async Task<IActionResult> Index(string searchString)
        {
            var livres = await _repository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var terme = searchString.Trim().ToLower();
                livres = livres.Where(l =>
                    (!string.IsNullOrEmpty(l.Titre) && l.Titre.ToLower().Contains(terme)) ||
                    (!string.IsNullOrEmpty(l.Auteur) && l.Auteur.ToLower().Contains(terme)) ||
                    (!string.IsNullOrEmpty(l.Isbn) && l.Isbn.ToLower().Contains(terme))
                );
            }

            return View(livres);
        }

        /// <summary>
        /// Affiche les détails d'un livre
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var livre = await _repository.GetByIdAsync(id);
            if (livre == null)
                return NotFound();

            return View(livre);
        }

        /// <summary>
        /// Affiche le formulaire de création
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Crée un nouveau livre
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titre,Auteur,AnneePublication,Isbn,QuantiteTotale,QuantiteDisponible")] Livre livre)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(livre);
                TempData["Success"] = "Livre créé avec succès";
                return RedirectToAction(nameof(Index));
            }
            return View(livre);
        }

        /// <summary>
        /// Affiche le formulaire d'édition
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var livre = await _repository.GetByIdAsync(id);
            if (livre == null)
                return NotFound();

            return View(livre);
        }

        /// <summary>
        /// Met à jour un livre
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Auteur,AnneePublication,Isbn,QuantiteTotale,QuantiteDisponible")] Livre livre)
        {
            if (id != livre.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(livre);
                TempData["Success"] = "Livre modifié avec succès";
                return RedirectToAction(nameof(Index));
            }
            return View(livre);
        }

        /// <summary>
        /// Affiche la confirmation de suppression
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var livre = await _repository.GetByIdAsync(id);
            if (livre == null)
                return NotFound();

            return View(livre);
        }

        /// <summary>
        /// Supprime un livre
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            TempData["Success"] = "Livre supprimé avec succès";
            return RedirectToAction(nameof(Index));
        }
    }
}
