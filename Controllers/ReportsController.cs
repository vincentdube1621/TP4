using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliothequeLIPAJOLI.Models;
using BibliothequeLIPAJOLI.Services;
using System.Globalization;

namespace BibliothequeLIPAJOLI.Controllers
{
    /// <summary>
    /// Contrôleur pour les rapports et statistiques
    /// </summary>
    public class ReportsController : Controller
    {
        private readonly BibliothequeContext _context;
        private readonly IBibliothequeService _bibliothequeService;

        public ReportsController(BibliothequeContext context, IBibliothequeService bibliothequeService)
        {
            _context = context;
            _bibliothequeService = bibliothequeService;
        }

        /// <summary>
        /// Affiche le tableau de bord avec les statistiques
        /// </summary>
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _bibliothequeService.GetStatistiquesAsync();
            return View(stats);
        }

        /// <summary>
        /// Affiche les prêts en retard
        /// </summary>
        public async Task<IActionResult> PretsEnRetard()
        {
            var pretsEnRetard = await _bibliothequeService.GetPretsEnRetardAsync();
            return View(pretsEnRetard);
        }

        /// <summary>
        /// Affiche les prêts actifs
        /// </summary>
        public async Task<IActionResult> PretsActifs()
        {
            var pretsActifs = await _bibliothequeService.GetPretsActifsAsync();
            return View(pretsActifs);
        }

        /// <summary>
        /// Affiche les livres les plus empruntés
        /// </summary>
        public async Task<IActionResult> LivresPopulaires()
        {
            var livresPopulaires = await _context.Livres
                .Include(l => l.Prets)
                .Select(l => new
                {
                    Livre = l,
                    NombreEmprunts = l.Prets!.Count
                })
                .OrderByDescending(x => x.NombreEmprunts)
                .Take(10)
                .ToListAsync();

            return View(livresPopulaires);
        }

        /// <summary>
        /// Affiche les usagers les plus actifs
        /// </summary>
        public async Task<IActionResult> UsagersActifs()
        {
            var usagersActifs = await _context.Usagers
                .Include(u => u.Prets)
                .Select(u => new
                {
                    Usager = u,
                    NombreEmprunts = u.Prets!.Count,
                    PretsActifs = u.Prets!.Count(p => p.EstActif)
                })
                .OrderByDescending(x => x.NombreEmprunts)
                .Take(10)
                .ToListAsync();

            return View(usagersActifs);
        }

        /// <summary>
        /// Génère un rapport d'activité pour une période donnée
        /// </summary>
        public async Task<IActionResult> RapportActivite(DateTime? dateDebut, DateTime? dateFin)
        {
            var debut = dateDebut ?? DateTime.Now.AddMonths(-1);
            var fin = dateFin ?? DateTime.Now;

            var rapport = new
            {
                Periode = new { Debut = debut, Fin = fin },
                NouveauxLivres = await _context.Livres.CountAsync(l => l.Id > 0), // Simplifié pour l'exemple
                NouveauxUsagers = await _context.Usagers.CountAsync(u => u.Id > 0), // Simplifié pour l'exemple
                PretsEffectues = await _context.Prets.CountAsync(p => p.DatePret >= debut && p.DatePret <= fin),
                RetoursEffectues = await _context.Prets.CountAsync(p => p.DateRetourReelle >= debut && p.DateRetourReelle <= fin),
                PretsEnRetard = await _context.Prets.CountAsync(p => p.EstEnRetard)
            };

            return View(rapport);
        }

        /// <summary>
        /// Exporte les données en CSV
        /// </summary>
        public async Task<IActionResult> ExportCsv(string type)
        {
            var csv = type.ToLower() switch
            {
                "livres" => await ExportLivresCsv(),
                "usagers" => await ExportUsagersCsv(),
                "prets" => await ExportPretsCsv(),
                _ => "Type d'export non supporté"
            };

            var fileName = $"export_{type}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
        }

        private async Task<string> ExportLivresCsv()
        {
            var livres = await _context.Livres.ToListAsync();
            var csv = "Titre,Auteur,ISBN,Année,Quantité Totale,Quantité Disponible\n";
            
            foreach (var livre in livres)
            {
                csv += $"\"{livre.Titre}\",\"{livre.Auteur}\",\"{livre.Isbn}\",{livre.AnneePublication},{livre.QuantiteTotale},{livre.QuantiteDisponible}\n";
            }
            
            return csv;
        }

        private async Task<string> ExportUsagersCsv()
        {
            var usagers = await _context.Usagers.ToListAsync();
            var csv = "Prénom,Nom,Courriel,Téléphone\n";
            
            foreach (var usager in usagers)
            {
                csv += $"\"{usager.Prenom}\",\"{usager.Nom}\",\"{usager.Courriel}\",\"{usager.Telephone ?? ""}\"\n";
            }
            
            return csv;
        }

        private async Task<string> ExportPretsCsv()
        {
            var prets = await _context.Prets
                .Include(p => p.Livre)
                .Include(p => p.Usager)
                .ToListAsync();
            
            var csv = "Livre,Usager,Date Prêt,Date Retour Prévue,Date Retour Réelle,Statut\n";
            
            foreach (var pret in prets)
            {
                csv += $"\"{pret.Livre.Titre}\",\"{pret.Usager.NomComplet}\",{pret.DatePret:yyyy-MM-dd},{pret.DateRetourPrevue:yyyy-MM-dd},{pret.DateRetourReelle?.ToString("yyyy-MM-dd") ?? ""},\"{pret.Statut}\"\n";
            }
            
            return csv;
        }
    }
}
