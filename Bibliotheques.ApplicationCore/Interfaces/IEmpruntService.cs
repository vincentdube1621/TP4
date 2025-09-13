using Bibliotheques.ApplicationCore.Entities;

namespace Bibliotheques.ApplicationCore.Interfaces
{
    /// <summary>
    /// Interface pour le service de gestion des emprunts
    /// </summary>
    public interface IEmpruntService
    {
        /// <summary>
        /// Vérifie si un usager peut emprunter un livre
        /// </summary>
        /// <param name="usagerId">Identifiant de l'usager</param>
        /// <param name="livreId">Identifiant du livre</param>
        Task<bool> PeutEmprunterLivreAsync(int usagerId, int livreId);

        /// <summary>
        /// Crée un nouvel emprunt
        /// </summary>
        /// <param name="usagerId">Identifiant de l'usager</param>
        /// <param name="livreId">Identifiant du livre</param>
        /// <param name="dureeJours">Durée de l'emprunt en jours</param>
        Task<Emprunt?> CreerEmpruntAsync(int usagerId, int livreId, int dureeJours = 10);

        /// <summary>
        /// Retourne un livre emprunté
        /// </summary>
        /// <param name="empruntId">Identifiant de l'emprunt</param>
        Task<bool> RetournerLivreAsync(int empruntId);

        /// <summary>
        /// Récupère tous les emprunts avec leurs relations
        /// </summary>
        Task<IEnumerable<Emprunt>> GetEmpruntsAvecRelationsAsync();

        /// <summary>
        /// Récupère un emprunt avec ses relations
        /// </summary>
        /// <param name="id">Identifiant de l'emprunt</param>
        Task<Emprunt?> GetEmpruntAvecRelationsAsync(int id);

        /// <summary>
        /// Récupère les emprunts en retard
        /// </summary>
        Task<IEnumerable<Emprunt>> GetEmpruntsEnRetardAsync();

        /// <summary>
        /// Récupère les emprunts actifs
        /// </summary>
        Task<IEnumerable<Emprunt>> GetEmpruntsActifsAsync();

        /// <summary>
        /// Supprime un emprunt (seulement si pas encore retourné)
        /// </summary>
        /// <param name="id">Identifiant de l'emprunt</param>
        Task<bool> SupprimerEmpruntAsync(int id);

        /// <summary>
        /// Vérifie et traite les défaillances pour un emprunt retourné
        /// </summary>
        /// <param name="emprunt">Emprunt retourné</param>
        /// <param name="joursLimite">Nombre de jours limite pour éviter la défaillance</param>
        Task TraiterDefaillanceAsync(Emprunt emprunt, int joursLimite);
    }
}
