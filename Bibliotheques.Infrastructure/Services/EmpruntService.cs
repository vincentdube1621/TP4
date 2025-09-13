using Microsoft.EntityFrameworkCore;
using Bibliotheques.ApplicationCore.Entities;
using Bibliotheques.ApplicationCore.Interfaces;
using Bibliotheques.Infrastructure.Data;

namespace Bibliotheques.Infrastructure.Services
{
    /// <summary>
    /// Service de gestion des emprunts
    /// </summary>
    public class EmpruntService : IEmpruntService
    {
        private readonly BibliothequeContext _context;
        private readonly IRepository<Emprunt> _empruntRepository;
        private readonly IRepository<Livre> _livreRepository;
        private readonly IRepository<Usager> _usagerRepository;

        public EmpruntService(
            BibliothequeContext context,
            IRepository<Emprunt> empruntRepository,
            IRepository<Livre> livreRepository,
            IRepository<Usager> usagerRepository)
        {
            _context = context;
            _empruntRepository = empruntRepository;
            _livreRepository = livreRepository;
            _usagerRepository = usagerRepository;
        }

        public async Task<bool> PeutEmprunterLivreAsync(int usagerId, int livreId)
        {
            // Vérifier si l'usager existe et n'est pas bloqué
            var usager = await _usagerRepository.GetByIdAsync(usagerId);
            if (usager == null || usager.EstBloque)
                return false;

            // Vérifier si le livre existe et est disponible
            var livre = await _livreRepository.GetByIdAsync(livreId);
            if (livre == null || !livre.EstDisponible)
                return false;

            // Vérifier si l'usager a déjà emprunté ce livre
            var empruntExistant = await _empruntRepository.FirstOrDefaultAsync(
                e => e.UsagerId == usagerId && e.LivreId == livreId && e.EstActif);
            if (empruntExistant != null)
                return false;

            // Vérifier la limite de 3 emprunts actifs
            var empruntsActifs = await _empruntRepository.CountAsync(
                e => e.UsagerId == usagerId && e.EstActif);
            if (empruntsActifs >= 3)
                return false;

            return true;
        }

        public async Task<Emprunt?> CreerEmpruntAsync(int usagerId, int livreId, int dureeJours = 10)
        {
            if (!await PeutEmprunterLivreAsync(usagerId, livreId))
                return null;

            try
            {
                var emprunt = new Emprunt
                {
                    UsagerId = usagerId,
                    LivreId = livreId,
                    DateEmprunt = DateTime.Now,
                    DateRetourPrevue = DateTime.Now.AddDays(dureeJours)
                };

                var empruntCree = await _empruntRepository.AddAsync(emprunt);

                // Décrémenter la quantité disponible
                var livre = await _livreRepository.GetByIdAsync(livreId);
                if (livre != null)
                {
                    livre.QuantiteDisponible--;
                    await _livreRepository.UpdateAsync(livre);
                }

                return empruntCree;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RetournerLivreAsync(int empruntId)
        {
            try
            {
                var emprunt = await _empruntRepository.GetByIdAsync(empruntId);
                if (emprunt == null || !emprunt.EstActif)
                    return false;

                emprunt.DateRetourReelle = DateTime.Now;

                // Incrémenter la quantité disponible
                var livre = await _livreRepository.GetByIdAsync(emprunt.LivreId);
                if (livre != null)
                {
                    livre.QuantiteDisponible++;
                    await _livreRepository.UpdateAsync(livre);
                }

                await _empruntRepository.UpdateAsync(emprunt);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Emprunt>> GetEmpruntsAvecRelationsAsync()
        {
            return await _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Usager)
                .OrderByDescending(e => e.DateEmprunt)
                .ToListAsync();
        }

        public async Task<Emprunt?> GetEmpruntAvecRelationsAsync(int id)
        {
            return await _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Usager)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Emprunt>> GetEmpruntsEnRetardAsync()
        {
            return await _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Usager)
                .Where(e => e.EstEnRetard)
                .OrderBy(e => e.DateRetourPrevue)
                .ToListAsync();
        }

        public async Task<IEnumerable<Emprunt>> GetEmpruntsActifsAsync()
        {
            return await _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Usager)
                .Where(e => e.EstActif)
                .OrderBy(e => e.DateRetourPrevue)
                .ToListAsync();
        }

        public async Task<bool> SupprimerEmpruntAsync(int id)
        {
            try
            {
                var emprunt = await _empruntRepository.GetByIdAsync(id);
                if (emprunt == null || emprunt.DateRetourReelle.HasValue)
                    return false;

                // Incrémenter la quantité disponible avant suppression
                var livre = await _livreRepository.GetByIdAsync(emprunt.LivreId);
                if (livre != null)
                {
                    livre.QuantiteDisponible++;
                    await _livreRepository.UpdateAsync(livre);
                }

                await _empruntRepository.DeleteAsync(emprunt);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task TraiterDefaillanceAsync(Emprunt emprunt, int joursLimite)
        {
            if (emprunt.DateRetourReelle == null)
                return;

            var dureeReelle = (emprunt.DateRetourReelle.Value - emprunt.DateEmprunt).Days;
            
            if (dureeReelle > joursLimite)
            {
                emprunt.EstDefaillance = true;
                
                // Incrémenter le compteur de défaillances de l'usager
                var usager = await _usagerRepository.GetByIdAsync(emprunt.UsagerId);
                if (usager != null)
                {
                    usager.NombreDefaillances++;
                    await _usagerRepository.UpdateAsync(usager);
                }

                await _empruntRepository.UpdateAsync(emprunt);
            }
        }
    }
}
