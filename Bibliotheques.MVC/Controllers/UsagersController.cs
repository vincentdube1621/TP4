using Microsoft.AspNetCore.Mvc;
using Bibliotheques.ApplicationCore.Entities;
using Bibliotheques.ApplicationCore.Interfaces;

namespace Bibliotheques.MVC.Controllers
{
    /// <summary>
    /// Contrôleur MVC pour la gestion des usagers (gestion locale)
    /// </summary>
    public class UsagersController : Controller
    {
        private readonly IRepository<Usager> _repository;

        public UsagersController(IRepository<Usager> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Affiche la liste des usagers
        /// </summary>
        public async Task<IActionResult> Index(string searchString)
        {
            var usagers = await _repository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var terme = searchString.Trim().ToLower();
                usagers = usagers.Where(u =>
                    (!string.IsNullOrEmpty(u.Prenom) && u.Prenom.ToLower().Contains(terme)) ||
                    (!string.IsNullOrEmpty(u.Nom) && u.Nom.ToLower().Contains(terme)) ||
                    (!string.IsNullOrEmpty(u.Courriel) && u.Courriel.ToLower().Contains(terme))
                );
            }

            return View(usagers);
        }

        /// <summary>
        /// Affiche les détails d'un usager
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var usager = await _repository.GetByIdAsync(id);
            if (usager == null)
                return NotFound();

            return View(usager);
        }

        /// <summary>
        /// Affiche le formulaire de création
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Crée un nouvel usager
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Prenom,Nom,Courriel,Telephone")] Usager usager)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(usager);
                TempData["Success"] = "Usager créé avec succès";
                return RedirectToAction(nameof(Index));
            }
            return View(usager);
        }

        /// <summary>
        /// Affiche le formulaire d'édition
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var usager = await _repository.GetByIdAsync(id);
            if (usager == null)
                return NotFound();

            return View(usager);
        }

        /// <summary>
        /// Met à jour un usager
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Prenom,Nom,Courriel,Telephone,NombreDefaillances")] Usager usager)
        {
            if (id != usager.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(usager);
                TempData["Success"] = "Usager modifié avec succès";
                return RedirectToAction(nameof(Index));
            }
            return View(usager);
        }

        /// <summary>
        /// Affiche la confirmation de suppression
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var usager = await _repository.GetByIdAsync(id);
            if (usager == null)
                return NotFound();

            return View(usager);
        }

        /// <summary>
        /// Supprime un usager
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            TempData["Success"] = "Usager supprimé avec succès";
            return RedirectToAction(nameof(Index));
        }
    }
}
