using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliothequeLIPAJOLI.Models;
using BibliothequeLIPAJOLI.Interfaces;
using System.Threading.Tasks;
using System;

namespace BibliothequeLIPAJOLI.Controllers
{
    public class PretsController : Controller
    {
        private readonly IRepository<Pret> _pretRepository;
        private readonly BibliothequeContext _context; // encore utilisé pour Include + SelectList

        public PretsController(IRepository<Pret> pretRepository, BibliothequeContext context)
        {
            _pretRepository = pretRepository;
            _context = context;
        }

        // GET: Prets
        public async Task<IActionResult> Index()
        {
            var prets = await _context.Prets
                .Include(p => p.Livre)
                .Include(p => p.Usager)
                .ToListAsync();

            return View(prets);
        }

        // GET: Prets/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pret = await _context.Prets
                .Include(p => p.Livre)
                .Include(p => p.Usager)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pret == null) return NotFound();
            return View(pret);
        }

        // GET: Prets/Create
        public IActionResult Create()
        {
            ViewData["LivreId"] = new SelectList(_context.Livres, "Id", "Titre");
            ViewData["UsagerId"] = new SelectList(_context.Usagers, "Id", "Nom");
            return View();
        }

        // POST: Prets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pret pret)
        {
            // Validation personnalisée
            var livre = await _context.Livres.FindAsync(pret.LivreId);
            if (livre == null)
            {
                ModelState.AddModelError("LivreId", "Livre introuvable.");
            }
            else if (livre.QuantiteDisponible <= 0)
            {
                ModelState.AddModelError("LivreId", "Ce livre n'est pas disponible.");
            }

            // Vérifier si l'usager a déjà emprunté ce livre
            var pretExistant = await _context.Prets
                .FirstOrDefaultAsync(p => p.UsagerId == pret.UsagerId && p.LivreId == pret.LivreId && p.DateRetourReelle == null);
            
            if (pretExistant != null)
            {
                ModelState.AddModelError("LivreId", "Cet usager a déjà emprunté ce livre.");
            }

            // Vérifier le nombre de prêts actifs par usager (limite de 3)
            var pretsActifsUsager = await _context.Prets
                .CountAsync(p => p.UsagerId == pret.UsagerId && p.DateRetourReelle == null);
            
            if (pretsActifsUsager >= 3)
            {
                ModelState.AddModelError("UsagerId", "Cet usager a atteint la limite de 3 prêts actifs.");
            }

            if (ModelState.IsValid)
            {
                // Définir la date de retour prévue
                pret.DateRetourPrevue = pret.DatePret.AddDays(10); // 10 jours par défaut
                
                await _pretRepository.AddAsync(pret);
                
                // Décrémenter la quantité disponible
                livre.QuantiteDisponible--;
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["LivreId"] = new SelectList(_context.Livres, "Id", "Titre", pret.LivreId);
            ViewData["UsagerId"] = new SelectList(_context.Usagers, "Id", "Nom", pret.UsagerId);
            return View(pret);
        }

        // GET: Prets/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var pret = await _pretRepository.GetByIdAsync(id);
            if (pret == null) return NotFound();

            ViewData["LivreId"] = new SelectList(_context.Livres, "Id", "Titre", pret.LivreId);
            ViewData["UsagerId"] = new SelectList(_context.Usagers, "Id", "Nom", pret.UsagerId);

            return View(pret);
        }

        // POST: Prets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pret pret)
        {
            if (id != pret.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _pretRepository.UpdateAsync(pret);
                return RedirectToAction(nameof(Index));
            }

            ViewData["LivreId"] = new SelectList(_context.Livres, "Id", "Titre", pret.LivreId);
            ViewData["UsagerId"] = new SelectList(_context.Usagers, "Id", "Nom", pret.UsagerId);

            return View(pret);
        }

        // GET: Prets/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var pret = await _context.Prets
                .Include(p => p.Livre)
                .Include(p => p.Usager)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pret == null) return NotFound();
            return View(pret);
        }

        // POST: Prets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _pretRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Prets/Retour/5
        public async Task<IActionResult> Retour(int id)
        {
            var pret = await _context.Prets
                .Include(p => p.Livre)
                .Include(p => p.Usager)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pret == null) return NotFound();
            if (pret.DateRetourReelle != null)
            {
                TempData["Error"] = "Ce prêt a déjà été retourné.";
                return RedirectToAction(nameof(Index));
            }

            return View(pret);
        }

        // POST: Prets/Retour/5
        [HttpPost, ActionName("Retour")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RetourConfirmed(int id)
        {
            var pret = await _context.Prets
                .Include(p => p.Livre)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pret == null) return NotFound();
            if (pret.DateRetourReelle != null)
            {
                TempData["Error"] = "Ce prêt a déjà été retourné.";
                return RedirectToAction(nameof(Index));
            }

            // Marquer le prêt comme retourné
            pret.DateRetourReelle = DateTime.Now;
            
            // Incrémenter la quantité disponible du livre
            pret.Livre.QuantiteDisponible++;
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Le livre '{pret.Livre.Titre}' a été retourné avec succès.";
            return RedirectToAction(nameof(Index));
        }
    }
}
