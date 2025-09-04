using Microsoft.AspNetCore.Mvc;
using BibliothequeLIPAJOLI.Models;
using BibliothequeLIPAJOLI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace BibliothequeLIPAJOLI.Controllers
{
    public class UsagersController : Controller
    {
        private readonly IRepository<Usager> _repository;

        public UsagersController(IRepository<Usager> repository)
        {
            _repository = repository;
        }

        // GET: Usagers
        public async Task<IActionResult> Index(string sortOrder)
        {
            var usagers = await _repository.GetAllAsync();

            usagers = sortOrder == "nom_desc"
                ? usagers.OrderByDescending(u => u.Nom)
                : usagers.OrderBy(u => u.Nom);

            return View(usagers);
        }

        // GET: Usagers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var usager = await _repository.GetByIdAsync(id);
            if (usager == null) return NotFound();

            return View(usager);
        }

        // GET: Usagers/Create
        public IActionResult Create() => View();

        // POST: Usagers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usager usager)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(usager);
                return RedirectToAction(nameof(Index));
            }
            return View(usager);
        }

        // GET: Usagers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var usager = await _repository.GetByIdAsync(id);
            if (usager == null) return NotFound();

            return View(usager);
        }

        // POST: Usagers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usager usager)
        {
            if (id != usager.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(usager);
                return RedirectToAction(nameof(Index));
            }
            return View(usager);
        }

        // GET: Usagers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var usager = await _repository.GetByIdAsync(id);
            if (usager == null) return NotFound();

            return View(usager);
        }

        // POST: Usagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
