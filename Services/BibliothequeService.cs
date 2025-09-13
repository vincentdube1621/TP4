using BibliothequeLIPAJOLI.Interfaces;
using BibliothequeLIPAJOLI.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliothequeLIPAJOLI.Services
{
    /// <summary>
    /// Service métier pour la gestion de la bibliothèque
    /// </summary>
    public interface IBibliothequeService
    {
        Task<bool> PeutEmprunterLivreAsync(int usagerId, int livreId);
        Task<bool> EmprunterLivreAsync(int usagerId, int livreId, int dureeJours = 10);
        Task<bool> RetournerLivreAsync(int pretId);
        Task<IEnumerable<Pret>> GetPretsEnRetardAsync();
        Task<IEnumerable<Pret>> GetPretsActifsAsync();
        Task<IEnumerable<Livre>> GetLivresDisponiblesAsync();
        Task<IEnumerable<Livre>> RechercherLivresAsync(string terme);
        Task<DashboardStats> GetStatistiquesAsync();
        Task<bool> ValiderIsbnUniqueAsync(string isbn, int? livreIdExclu = null);
    }

    /// <summary>
    /// Implémentation du service métier de la bibliothèque
    /// </summary>
    public class BibliothequeService : IBibliothequeService
    {
        private readonly BibliothequeContext _context;
        private readonly ILoggingService _loggingService;
        private readonly IParametresPret _parametresPret;

        public BibliothequeService(
            BibliothequeContext context, 
            ILoggingService loggingService,
            IParametresPret parametresPret)
        {
            _context = context;
            _loggingService = loggingService;
            _parametresPret = parametresPret;
        }

        public async Task<bool> PeutEmprunterLivreAsync(int usagerId, int livreId)
        {
            // Vérifier si l'usager existe
            var usager = await _context.Usagers.FindAsync(usagerId);
            if (usager == null) return false;

            // Vérifier si le livre existe et est disponible
            var livre = await _context.Livres.FindAsync(livreId);
            if (livre == null || !livre.EstDisponible) return false;

            // Vérifier si l'usager a déjà emprunté ce livre
            var pretExistant = await _context.Prets
                .FirstOrDefaultAsync(p => p.UsagerId == usagerId && p.LivreId == livreId && p.EstActif);
            if (pretExistant != null) return false;

            // Vérifier la limite de prêts actifs (3 maximum)
            var pretsActifs = await _context.Prets
                .CountAsync(p => p.UsagerId == usagerId && p.EstActif);
            if (pretsActifs >= 3) return false;

            return true;
        }

        public async Task<bool> EmprunterLivreAsync(int usagerId, int livreId, int dureeJours = 10)
        {
            if (!await PeutEmprunterLivreAsync(usagerId, livreId))
                return false;

            try
            {
                var pret = new Pret
                {
                    UsagerId = usagerId,
                    LivreId = livreId,
                    DatePret = DateTime.Now,
                    DateRetourPrevue = DateTime.Now.AddDays(dureeJours)
                };

                _context.Prets.Add(pret);

                // Décrémenter la quantité disponible
                var livre = await _context.Livres.FindAsync(livreId);
                if (livre != null)
                {
                    livre.QuantiteDisponible--;
                }

                await _context.SaveChangesAsync();

                var usager = await _context.Usagers.FindAsync(usagerId);
                _loggingService.LogPretCree(pret.Id, livre?.Titre ?? "", usager?.NomComplet ?? "");

                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogErreur($"Erreur lors de l'emprunt du livre {livreId} par l'usager {usagerId}", ex);
                return false;
            }
        }

        public async Task<bool> RetournerLivreAsync(int pretId)
        {
            try
            {
                var pret = await _context.Prets
                    .Include(p => p.Livre)
                    .Include(p => p.Usager)
                    .FirstOrDefaultAsync(p => p.Id == pretId);

                if (pret == null || !pret.EstActif)
                    return false;

                pret.DateRetourReelle = DateTime.Now;
                pret.Livre.QuantiteDisponible++;

                await _context.SaveChangesAsync();

                _loggingService.LogPretRetourne(pret.Id, pret.Livre.Titre, pret.Usager.NomComplet);

                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogErreur($"Erreur lors du retour du prêt {pretId}", ex);
                return false;
            }
        }

        public async Task<IEnumerable<Pret>> GetPretsEnRetardAsync()
        {
            return await _context.Prets
                .Include(p => p.Livre)
                .Include(p => p.Usager)
                .Where(p => p.EstEnRetard)
                .OrderBy(p => p.DateRetourPrevue)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pret>> GetPretsActifsAsync()
        {
            return await _context.Prets
                .Include(p => p.Livre)
                .Include(p => p.Usager)
                .Where(p => p.EstActif)
                .OrderBy(p => p.DateRetourPrevue)
                .ToListAsync();
        }

        public async Task<IEnumerable<Livre>> GetLivresDisponiblesAsync()
        {
            return await _context.Livres
                .Where(l => l.EstDisponible)
                .OrderBy(l => l.Titre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Livre>> RechercherLivresAsync(string terme)
        {
            if (string.IsNullOrWhiteSpace(terme))
                return await _context.Livres.ToListAsync();

            var termeLower = terme.ToLower();
            return await _context.Livres
                .Where(l => l.Titre.ToLower().Contains(termeLower) ||
                           l.Auteur.ToLower().Contains(termeLower) ||
                           l.Isbn.Contains(terme))
                .OrderBy(l => l.Titre)
                .ToListAsync();
        }

        public async Task<DashboardStats> GetStatistiquesAsync()
        {
            return new DashboardStats
            {
                TotalLivres = await _context.Livres.CountAsync(),
                TotalUsagers = await _context.Usagers.CountAsync(),
                PretsActifs = await _context.Prets.CountAsync(p => p.EstActif),
                PretsEnRetard = await _context.Prets.CountAsync(p => p.EstEnRetard),
                LivresDisponibles = await _context.Livres.SumAsync(l => l.QuantiteDisponible)
            };
        }

        public async Task<bool> ValiderIsbnUniqueAsync(string isbn, int? livreIdExclu = null)
        {
            var query = _context.Livres.Where(l => l.Isbn == isbn);
            
            if (livreIdExclu.HasValue)
                query = query.Where(l => l.Id != livreIdExclu.Value);

            return !await query.AnyAsync();
        }
    }
}
