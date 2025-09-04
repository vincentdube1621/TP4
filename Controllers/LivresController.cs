using Microsoft.AspNetCore.Mvc;
using BibliothequeLIPAJOLI.Models;
using BibliothequeLIPAJOLI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace BibliothequeLIPAJOLI.Controllers
{
    public class LivresController : Controller
    {
        private readonly IRepository<Livre> _repository;

        public LivresController(IRepository<Livre> repository)
        {
            _repository = repository;
        }

        // GET: Livres
        public async Task<IActionResult> Index(string searchString)
        {
            var livres = await _repository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                livres = livres.Where(l => l.Titre.ToLower().Contains(searchString.ToLower()));
            }

            return View(livres);
        }

        // GET: Livres/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var livre = await _repository.GetByIdAsync(id);
            if (livre == null) return NotFound();

            return View(livre);
        }

        // GET: Livres/Create
        public IActionResult Create() => View();

        // POST: Livres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Livre livre)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(livre);
                return RedirectToAction(nameof(Index));
            }
            return View(livre);
        }

        // GET: Livres/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var livre = await _repository.GetByIdAsync(id);
            if (livre == null) return NotFound();

            return View(livre);
        }

        // POST: Livres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Livre livre)
        {
            if (id != livre.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(livre);
                return RedirectToAction(nameof(Index));
            }
            return View(livre);
        }

        // GET: Livres/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var livre = await _repository.GetByIdAsync(id);
            if (livre == null) return NotFound();

            return View(livre);
        }

        // POST: Livres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
