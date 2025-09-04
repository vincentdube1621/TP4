using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliothequeLIPAJOLI.Models;
using BibliothequeLIPAJOLI.Interfaces;
using System.Threading.Tasks;

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
            if (ModelState.IsValid)
            {
                await _pretRepository.AddAsync(pret);
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
    }
}
